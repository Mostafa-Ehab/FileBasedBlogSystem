using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlogSystem.Shared.Mappings;
using BlogSystem.Features.Categories.CreateCategory;
using BlogSystem.Features.Categories.CreateCategory.DTOs;
using BlogSystem.Features.Categories.CreateCategory.Validators;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Categories.GetCategory;
using BlogSystem.Features.Posts.CreatePost;
using BlogSystem.Features.Posts.CreatePost.DTOs;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.Get;
using BlogSystem.Features.Posts.RSS;
using BlogSystem.Features.Posts.SchedulePost;
using BlogSystem.Features.Posts.UpdatePost;
using BlogSystem.Features.Posts.UpdatePost.DTOs;
using BlogSystem.Features.Posts.UpdatePost.Validators;
using BlogSystem.Features.Tags.CreateTag;
using BlogSystem.Features.Tags.CreateTag.DTOs;
using BlogSystem.Features.Tags.CreateTag.Validators;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Features.Tags.GetTag;
using BlogSystem.Features.Users.CreateUser;
using BlogSystem.Features.Users.CreateUser.DTOs;
using BlogSystem.Features.Users.CreateUser.Validators;
using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.Login;
using BlogSystem.Features.Users.Login.DTOs;
using BlogSystem.Features.Users.Login.Validators;
using BlogSystem.Infrastructure.ImageService;
using BlogSystem.Infrastructure.MarkdownService;
using BlogSystem.Infrastructure.Scheduling;
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

namespace BlogSystem.Shared.Extensions
{
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

            // Configure AutoMapper
            services.AddAutoMapper(config =>
            {
                config.AddProfile<PostMappingProfile>();
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
                            Encoding.ASCII.GetBytes(services.BuildServiceProvider().GetRequiredService<IConfiguration>()["JWT_SecretKey"] ?? throw new InvalidOperationException("JWT_SecretKey is not configured"))
                        ),
                    };
                });

            // Configure Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"));
                options.AddPolicy("Editor", policy => policy.RequireClaim("Role", "Editor"));
            });

            // Register ImageSharp services
            services.AddImageSharp()
                .AddProvider<PostImageProvider>()
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
            });

            services.AddSingleton<AuthHelper>();

            services.AddSingleton<SlugResolver>();
            services.AddSingleton<UserResolver>();

            services.AddSingleton<MarkdownService>();
            services.AddSingleton<PostImageProvider>();
            services.AddSingleton<IScheduler, HangfireScheduler>();

            services.AddValidators();
            services.AddCategoryServices();
            services.AddPostServices();
            services.AddTagServices();
            services.AddUserServices();

            return services;
        }

        private static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<LoginRequestDTO>, LoginRequestValidator>();
            services.AddScoped<IValidator<CreateUserRequestDTO>, CreateUserRequestValidator>();
            services.AddScoped<IValidator<UpdatePostRequestDTO>, UpdatePostRequestValidator>();
            services.AddScoped<IValidator<CreateCategoryRequestDTO>, CreateCategoryRequestValidator>();
            services.AddScoped<IValidator<CreateTagRequestDTO>, CreateTagRequestValidator>();
            return services;
        }

        private static IServiceCollection AddPostServices(this IServiceCollection services)
        {
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IGetPostHandler, GetPostHandler>();
            services.AddScoped<ICreatePostHandler, CreatePostHandler>();
            services.AddScoped<IUpdatePostHandler, UpdatePostHandler>();
            services.AddScoped<IRSSHandler, RSSHandler>();
            services.AddScoped<ISchedulePostHandler, SchedulePostHandler>();
            services.AddScoped<PublishPostService>();
            return services;
        }

        private static IServiceCollection AddCategoryServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IGetCategoryHandler, GetCategoryHandler>();
            services.AddScoped<ICreateCategoryHandler, CreateCategoryHandler>();
            return services;
        }

        private static IServiceCollection AddTagServices(this IServiceCollection services)
        {
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IGetTagHandler, GetTagHandler>();
            services.AddScoped<ICreateTagHandler, CreateTagHandler>();
            return services;
        }

        private static IServiceCollection AddUserServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILoginHandler, LoginHandler>();
            services.AddScoped<ICreateUserHandler, CreateUserHandler>();
            return services;
        }
    }
}