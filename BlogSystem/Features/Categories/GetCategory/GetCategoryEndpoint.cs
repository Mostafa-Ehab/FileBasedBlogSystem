using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Categories.GetCategory
{
    public static class GetCategoryEndpoint
    {
        public static void MapGetAllCategoriesEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/categories", async (IGetCategoryHandler handler) =>
            {
                var categories = await handler.GetAllCategoriesAsync();
                return Results.Ok(categories);
            })
            .WithName("GetAllCategories")
            .Produces<Category[]>(StatusCodes.Status200OK);
        }

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