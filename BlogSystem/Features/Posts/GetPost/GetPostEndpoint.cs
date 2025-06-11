using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Posts.Get
{
    public static class GetPostEndpoint
    {
        public static void MapGetPostEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/{slug}", async (string slug, IGetPostHandler handler) =>
            {
                var post = await handler.GetPostAsync(slug);
                return Results.Ok(post);
            })
            .WithName("GetPostBySlug")
            .WithTags("Posts")
            .Produces<Post>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}
