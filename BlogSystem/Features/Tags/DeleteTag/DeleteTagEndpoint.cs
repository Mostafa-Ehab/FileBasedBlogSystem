namespace BlogSystem.Features.Tags.DeleteTag;

public static class DeleteTagEndpoint
{
    public static void MapDeleteTagEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/{slug}", async (string slug, IDeleteTagHandler handler) =>
        {
            await handler.DeleteTagAsync(slug);
            return Results.NoContent();
        })
        .WithName("DeleteTag")
        .RequireAuthorization("Admin")
        .WithTags("Tags")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
