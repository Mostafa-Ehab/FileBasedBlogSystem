using System.Text.Json;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Users.CreateUser;
using BlogSystem.Features.Users.CreateUser.DTOs;
using BlogSystem.Features.Users.CreateUser.Validators;
using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.Login;
using BlogSystem.Features.Users.Login.DTOs;
using BlogSystem.Features.Users.Login.Validators;
using BlogSystem.Shared.Helpers;
using BlogSystemTest;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class UnitTestBase : IAsyncLifetime
{
    protected readonly string _testContentPath = Path.Combine(Directory.GetCurrentDirectory(), "TestContent");
    protected IServiceScope _scope;

    protected const int DelayMilliseconds = 1000; // Delay after seeding content

    public async Task InitializeAsync()
    {
        // Create a new service scope for each test
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<UnitTestBase>()
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        SeedContent();

        // Register Helpers and Resolvers
        services.AddSingleton<AuthHelper>();
        services.AddSingleton<UserResolver>();
        services.AddSingleton<SlugResolver>();

        // Register Validators
        services.AddScoped<IValidator<CreateUserRequestDTO>, CreateUserRequestValidator>();
        services.AddScoped<IValidator<LoginRequestDTO>, LoginRequestValidator>();

        // Register Repositories
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Register Handlers
        services.AddScoped<ICreateUserHandler, CreateUserHandler>();
        services.AddScoped<ILoginHandler, LoginHandler>();

        var provider = services.BuildServiceProvider();
        _scope = provider.CreateScope();
    }

    public async Task DisposeAsync()
    {
        // Delay to ensure all operations are completed before disposing
        await Task.Delay(DelayMilliseconds);
        _scope?.Dispose();
    }

    private static void SeedContent()
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