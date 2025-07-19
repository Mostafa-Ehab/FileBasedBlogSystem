namespace BlogSystem.Features.Categories.DeleteCategory;

public static class DeleteCategoryEndpoint
{
    public static void MapDeleteCategoryEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/{slug}", async (string slug, IDeleteCategoryHandler handler) =>
        {
            await handler.DeleteCategoryAsync(slug);
            return Results.NoContent();
        })
        .WithName("DeleteCategory")
        .RequireAuthorization("Admin")
        .WithTags("Categories")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
