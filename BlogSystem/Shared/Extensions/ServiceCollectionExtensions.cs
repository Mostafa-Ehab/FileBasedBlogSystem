using System.Text.Json;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Categories.GetCategory;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.Get;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Features.Tags.GetTag;
using BlogSystem.Features.Users.CreateUser;
using BlogSystem.Features.Users.CreateUser.DTOs;
using BlogSystem.Features.Users.CreateUser.Validators;
using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.Login;
using BlogSystem.Features.Users.Login.DTOs;
using BlogSystem.Features.Users.Login.Validators;
using BlogSystem.Shared.Helpers;
using FluentValidation;

namespace BlogSystem.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            services.AddSingleton<AuthHelper>();

            services.AddSingleton<SlugResolver>();
            services.AddSingleton<UserResolver>();

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
            return services;
        }

        private static IServiceCollection AddPostServices(this IServiceCollection services)
        {
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IGetPostHandler, GetPostHandler>();
            return services;
        }

        private static IServiceCollection AddCategoryServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IGetCategoryHandler, GetCategoryHandler>();
            return services;
        }

        private static IServiceCollection AddTagServices(this IServiceCollection services)
        {
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IGetTagHandler, GetTagHandler>();
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