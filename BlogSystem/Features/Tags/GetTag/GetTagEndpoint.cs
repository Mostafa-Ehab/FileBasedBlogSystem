using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Tags.GetTag
{
    public static class GetTagEndpoint
    {
        public static void MapGetAllTagsEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/tags", async (IGetTagHandler handler) =>
            {
                return Results.Ok(await handler.GetAllTagsAsync());
            })
            .WithName("GetAllTags")
            .WithTags("Tags")
            .Produces<Tag[]>(StatusCodes.Status200OK);
        }

        public static void MapGetTagEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/tags/{slug}", async (string slug, IGetTagHandler handler) =>
            {
                return Results.Ok(await handler.GetTagAsync(slug));
            })
            .WithName("GetTag")
            .WithTags("Tags")
            .Produces<Tag>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }

        public static void MapGetPostsByTagEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/tags/{slug}/posts", async (string slug, IGetTagHandler handler) =>
            {
                return Results.Ok(await handler.GetPostsByTagAsync(slug));
            })
            .WithName("GetPostsByTag")
            .WithTags("Tags")
            .Produces<Post[]>(StatusCodes.Status200OK);
        }
    }
}
