using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Models;

namespace BlogSystem.Features.Categories.GetCategory
{
    public static class GetCategoryEndpoint
    {
        public static void MapGetCategoryEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/categories/{slug}", async (string slug, IGetCategoryHandler handler) =>
            {
                var category = await handler.GetCategoryAsync(slug);
                return Results.Ok(category);
            })
            .WithName("GetCategoryBySlug")
            .Produces<Category>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }

        public static void MapGetPostsByCategoryEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/categories/{slug}/posts", async (string slug, IGetCategoryHandler handler) =>
            {
                var posts = await handler.GetPostsByCategoryAsync(slug);
                return Results.Ok(posts);
            })
            .WithName("GetPostsByCategory")
            .Produces<Post[]>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}