using System.Text.Json;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Categories.GetCategory;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.Get;

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

            services.AddCategoryServices();
            services.AddPostServices();
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
    }
}