using System.Text.Json;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Categories.GetCategory;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.Get;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Features.Tags.GetTag;
using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.Login;
using BlogSystem.Shared.Helpers;

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
            return services;
        }
    }
}