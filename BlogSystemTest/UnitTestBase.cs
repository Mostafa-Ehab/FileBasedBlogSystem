using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Users.CreateUser;
using BlogSystem.Features.Users.CreateUser.Validators;
using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.Login;
using BlogSystem.Features.Users.Login.Validators;
using BlogSystem.Shared.Helpers;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

public class UnitTestBase
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly AuthHelper _authHelper;

    public UnitTestBase()
    {
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        _authHelper = new AuthHelper();
    }

    public UserRepository CreateUserRepository()
    {
        var userResolver = new UserResolver(_jsonSerializerOptions);
        var postRepository = CreatePostRepository();
        return new UserRepository(_jsonSerializerOptions, userResolver, postRepository);
    }

    public PostRepository CreatePostRepository()
    {
        var slugResolver = new SlugResolver(_jsonSerializerOptions);
        return new PostRepository(_jsonSerializerOptions, slugResolver);
    }

    public CategoryRepository CreateCategoryRepository()
    {
        return new CategoryRepository(_jsonSerializerOptions);
    }

    public LoginHandler CreateLoginHandler()
    {
        var userRepository = CreateUserRepository();
        var loginValidator = new LoginRequestValidator();
        return new LoginHandler(userRepository, _authHelper, loginValidator);
    }

    public CreateUserHandler CreateCreateUserHandler()
    {
        var userRepository = CreateUserRepository();
        var createUserValidator = new CreateUserRequestValidator();
        return new CreateUserHandler(userRepository, _authHelper, createUserValidator);
    }

    public static void SeedContent()
    {
        Directory.SetCurrentDirectory(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "BlogSystemTest")
        );

        var sourcePath = Path.Combine("..", "BlogSystem", "SeedData");
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