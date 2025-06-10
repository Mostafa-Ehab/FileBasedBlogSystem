using BlogSystem.Features.Posts.Get;
using BlogSystem.Features.Categories.GetCategory;

namespace BlogSystem.Shared.Extensions
{
    public static class MinimalApiExtensions
    {
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPostEndpoints();
            app.MapCategoryEndpoints();

            return app;
        }
        
        private static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGetPostEndpoint();

            return app;
        }
        
        private static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGetCategoryEndpoint();
            app.MapGetPostsByCategoryEndpoint();

            return app;
        }
    }
}