using BlogSystem.Domain.Entities;
using BlogSystem.Infrastructure.SearchEngineService;
using BlogSystem.Shared.Helpers;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DevTools;

internal class Program
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        IgnoreReadOnlyProperties = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            ShowUsage();
            return;
        }

        var command = args[0].ToLower();
        switch (command)
        {
            case "hash":
                if (args.Length != 2)
                {
                    Console.WriteLine("Usage: DevTools.exe hash <password>");
                    return;
                }
                HashPassword(args[1]);
                break;

            case "seed":
                SeedContent();
                break;

            case "index":
                IndexContent();
                break;

            default:
                ShowUsage();
                break;
        }
    }

    private static void ShowUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  DevTools.exe hash <password> - Generate password hash");
        Console.WriteLine("  DevTools.exe seed           - Copy seed data to Content folder");
        Console.WriteLine("  DevTools.exe index          - Index all blog posts");
    }

    private static void HashPassword(string password)
    {
        var hashedPassword = AuthHelper.HashPassword(password);
        Console.WriteLine($"Hashed password: {hashedPassword}");
    }

    private static void SeedContent()
    {
        try
        {
            Console.WriteLine("Seeding content...");
            Directory.SetCurrentDirectory("BlogSystem");

            var sourcePath = Path.Combine("SeedData");
            var targetPath = Path.Combine("Content");

            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine("Error: SeedData folder not found");
                return;
            }

            if (Directory.Exists(targetPath))
            {
                Console.WriteLine("Content folder already exists. It will be overwritten.");
                Directory.Delete(targetPath, true);
            }
            Directory.CreateDirectory(targetPath);

            CopyDirectory(sourcePath, targetPath);
            Console.WriteLine("Content seeded successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding content: {ex.Message}");
        }
    }

    private static void CopyDirectory(string sourceDir, string targetDir)
    {
        Directory.CreateDirectory(targetDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);
            var destFile = Path.Combine(targetDir, fileName);
            File.Copy(file, destFile, true);
        }

        foreach (var directory in Directory.GetDirectories(sourceDir))
        {
            var dirName = Path.GetFileName(directory);
            var destDir = Path.Combine(targetDir, dirName);
            CopyDirectory(directory, destDir);
        }
    }

    private static void IndexContent()
    {
        try
        {
            Console.WriteLine("Indexing content...");
            Directory.SetCurrentDirectory("BlogSystem");

            var indexPath = Path.Combine("Content", "index");
            if (Directory.Exists(indexPath))
            {
                Directory.Delete(indexPath, true);
            }

            var postSearchEngineService = new PostSearchEngineService();
            var path = Path.Combine("Content", "posts");
            var postFiles = Directory.GetDirectories(path)
                .Select(dir => Path.Combine(dir, "meta.json"))
                .Where(File.Exists)
                .OrderByDescending(File.GetLastWriteTime);

            var posts = postFiles
                .Select(file => JsonSerializer.Deserialize<Post>(File.ReadAllText(file), _jsonSerializerOptions))
                .Where(post => post != null)
                .Select(post =>
                {
                    post!.Content = File.ReadAllText(Path.Combine(path, post.Id, "content.md"));
                    return post;
                })
                .ToArray();

            foreach (var post in posts)
            {
                Console.WriteLine($"Indexing post: {post.Title}");
                postSearchEngineService.IndexDocumentAsync(post);
            }

            Console.WriteLine("Content indexed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error indexing content: {ex.Message}");
        }
    }
}