using BlogSystem.Shared.Helpers;
using Microsoft.Extensions.Configuration;

namespace DevTools
{
    internal class Program
    {
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
        }

        private static void HashPassword(string password)
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();
            var configuration = builder.Build();

            try
            {
                AuthHelper authHelper = new AuthHelper(configuration);
                var hashedPassword = authHelper.HashPassword(password);
                Console.WriteLine($"Hashed password: {hashedPassword}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void SeedContent()
        {
            try
            {
                var sourcePath = Path.Combine("BlogSystem", "SeedData");
                var targetPath = Path.Combine("BlogSystem", "Content");

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
    }
}