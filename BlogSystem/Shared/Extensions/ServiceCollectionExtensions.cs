using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.CreateCategory;
using BlogSystem.Features.Categories.CreateCategory.DTOs;
using BlogSystem.Features.Categories.CreateCategory.Validators;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Categories.DeleteCategory;
using BlogSystem.Features.Categories.GetCategory;
using BlogSystem.Features.Categories.UpdateCategory;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.GetPost;
using BlogSystem.Features.Posts.PostManagement;
using BlogSystem.Features.Posts.PostManagement.States;
using BlogSystem.Features.Posts.RSS;
using BlogSystem.Features.Tags.CreateTag;
using BlogSystem.Features.Tags.CreateTag.DTOs;
using BlogSystem.Features.Tags.CreateTag.Validators;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Features.Tags.DeleteTag;
using BlogSystem.Features.Tags.GetTag;
using BlogSystem.Features.Tags.UpdateTag;
using BlogSystem.Features.Users.CreateUser;
using BlogSystem.Features.Users.CreateUser.DTOs;
using BlogSystem.Features.Users.CreateUser.Validators;
using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.DeleteUser;
using BlogSystem.Features.Users.GetUser;
using BlogSystem.Features.Users.Login;
using BlogSystem.Features.Users.Login.DTOs;
using BlogSystem.Features.Users.Login.Validators;
using BlogSystem.Features.Users.UpdateUser;
using BlogSystem.Features.Users.UpdateUser.DTOs;
using BlogSystem.Features.Users.UpdateUser.Validators;
using BlogSystem.Infrastructure.ImageService;
using BlogSystem.Infrastructure.MarkdownService;
using BlogSystem.Infrastructure.Scheduling;
using BlogSystem.Infrastructure.SearchEngineService;
using BlogSystem.Shared.Exceptions.Users;
using BlogSystem.Shared.Helpers;
using FluentValidation;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Middleware;
using SixLabors.ImageSharp.Web.Providers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlogSystem.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Configure JSON serialization options
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        // Register Authentication Service
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
            options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(
                            Environment.GetEnvironmentVariable("JWT_SecretKey") ??
                                throw new InvalidOperationException("JWT_SecretKey is not configured")
                        )
                    ),
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            throw new NotAuthorizedException("Token has expired", 40101);
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        if (!context.Response.HasStarted)
                        {
                            context.HandleResponse();
                            throw new NotAuthorizedException("Unauthorized access");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        // Configure Authorization
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"));
            options.AddPolicy("Editor", policy => policy.RequireAssertion(context =>
                context.User.HasClaim(c => c.Type == "Role" && (c.Value == "Admin" || c.Value == "Editor"))));
            options.AddPolicy("Author", policy => policy.RequireAssertion(context =>
                context.User.HasClaim(c => c.Type == "Role" && (c.Value == "Admin" || c.Value == "Editor" || c.Value == "Author"))));
        });

        // Register ImageSharp services
        services.AddImageSharp()
            .AddProvider<PostImageProvider>()
            .AddProvider<UserImageProvider>()
            .RemoveProvider<PhysicalFileSystemProvider>()
            .AddProvider<PhysicalFileSystemProvider>()
            .Configure<PhysicalFileSystemCacheOptions>(options =>
            {
                options.CacheRootPath = Path.Combine("Content", "cache");
            })
            .Configure<ImageSharpMiddlewareOptions>(options =>
            {
                options.OnPrepareResponseAsync = context =>
                {
                    context.Response.Headers.CacheControl = "public, max-age=31536000"; // Cache for 1 year
                    return Task.CompletedTask;
                };
            });

        // Register Hangfire for scheduling
        services.AddHangfire(config =>
        {
            config.UseMemoryStorage();
        });
        services.AddHangfireServer();

        services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            IgnoreReadOnlyProperties = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        });

#if DEBUG
        services.AddSassCompiler();
#endif

        services.AddSingleton<AuthHelper>();

        services.AddSingleton<SlugResolver>();
        services.AddSingleton<UserResolver>();

        services.AddSingleton<MarkdownService>();
        services.AddSingleton<PostImageProvider>();
        services.AddSingleton<UserImageProvider>();
        services.AddSingleton<ISearchEngineService<Post>, PostSearchEngineService>();
        services.AddSingleton<IScheduler, HangfireScheduler>();

        services.AddCategoryServices();
        services.AddPostServices();
        services.AddTagServices();
        services.AddUserServices();

        return services;
    }

    private static IServiceCollection AddPostServices(this IServiceCollection services)
    {
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IGetPostHandler, GetPostHandler>();
        services.AddScoped<IPostManagementHandler, PostManagementHandler>();
        services.AddScoped<IRSSHandler, RSSHandler>();
        services.AddScoped<PublishPostService>();

        services.AddScoped<DraftState>();
        services.AddScoped<ScheduledState>();
        services.AddScoped<PublishedState>();
        return services;
    }

    private static IServiceCollection AddCategoryServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateCategoryRequestDTO>, CreateCategoryRequestValidator>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IGetCategoryHandler, GetCategoryHandler>();
        services.AddScoped<ICreateCategoryHandler, CreateCategoryHandler>();
        services.AddScoped<IUpdateCategoryHandler, UpdateCategoryHandler>();
        services.AddScoped<IDeleteCategoryHandler, DeleteCategoryHandler>();
        return services;
    }

    private static IServiceCollection AddTagServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateTagRequestDTO>, CreateTagRequestValidator>();

        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IGetTagHandler, GetTagHandler>();
        services.AddScoped<ICreateTagHandler, CreateTagHandler>();
        services.AddScoped<IUpdateTagHandler, UpdateTagHandler>();
        services.AddScoped<IDeleteTagHandler, DeleteTagHandler>();
        return services;
    }

    private static IServiceCollection AddUserServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<LoginRequestDTO>, LoginRequestValidator>();
        services.AddScoped<IValidator<CreateUserRequestDTO>, CreateUserRequestValidator>();
        services.AddScoped<IValidator<UpdateUserRequestDTO>, UpdateUserRequestValidator>();
        services.AddScoped<IValidator<UpdateProfileInfoRequestDTO>, UpdateProfileInfoRequestValidator>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ILoginHandler, LoginHandler>();
        services.AddScoped<ICreateUserHandler, CreateUserHandler>();
        services.AddScoped<IGetUserHandler, GetUserHandler>();
        services.AddScoped<IUpdateUserHandler, UpdateUserHandler>();
        services.AddScoped<IDeleteUserHandler, DeleteUserHandler>();
        return services;
    }
}