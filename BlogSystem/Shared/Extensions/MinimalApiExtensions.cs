using BlogSystem.Features.Posts.Get;

namespace BlogSystem.Shared.Extensions
{
    public static class MinimalApiExtensions
    {
        public static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGetPostEndpoint();
            
            return app;
        }
    }
}