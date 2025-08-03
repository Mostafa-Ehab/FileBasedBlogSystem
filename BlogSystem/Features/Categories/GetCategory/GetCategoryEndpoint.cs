using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Categories.GetCategory;

public static class GetCategoryEndpoint
{
    public static void MapGetAllCategoriesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (IGetCategoryHandler handler) =>
        {
            var categories = await handler.GetAllCategoriesAsync();
            return Results.Ok(categories);
        })
        .WithName("GetAllCategories")
        .WithTags("Categories")
        .Produces<Category[]>(StatusCodes.Status200OK);
    }

    public static void MapGetCategoryEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{slug}", async (string slug, IGetCategoryHandler handler) =>
        {
            var category = await handler.GetCategoryAsync(slug);
            return Results.Ok(category);
        })
        .WithName("GetCategoryBySlug")
        .WithTags("Categories")
        .Produces<Category>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }

    public static void MapGetPostsByCategoryEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{slug}/posts", async (string slug, IGetCategoryHandler handler, int page = 1, int pageSize = 10) =>
        {
            var posts = await handler.GetPostsByCategoryAsync(slug, page, pageSize);
            return Results.Ok(posts);
        })
        .WithName("GetPostsByCategory")
        .WithTags("Categories")
        .Produces<Post[]>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}