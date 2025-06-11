using BlogSystem.Features.Posts.Get;
using BlogSystem.Features.Categories.GetCategory;
using BlogSystem.Features.Tags.GetTag;

namespace BlogSystem.Shared.Extensions
{
    public static class MinimalApiExtensions
    {
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGroup("/posts").MapPostEndpoints();
            app.MapGroup("/categories").MapCategoryEndpoints();
            app.MapGroup("/tags").MapTagEndpoints();

            return app;
        }
        
        private static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGetPostEndpoint();

            return app;
        }
        
        private static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGetAllCategoriesEndpoint();
            app.MapGetCategoryEndpoint();
            app.MapGetPostsByCategoryEndpoint();

            return app;
        }

        private static IEndpointRouteBuilder MapTagEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGetAllTagsEndpoint();
            app.MapGetTagEndpoint();
            app.MapGetPostsByTagEndpoint();

            return app;
        }
    }
}