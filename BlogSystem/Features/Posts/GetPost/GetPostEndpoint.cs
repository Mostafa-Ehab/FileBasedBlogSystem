using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Posts.Get
{
    public static class GetPostEndpoint
    {
        public static void MapGetPostEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/p/{slug}", async (string slug, IGetPostHandler handler) =>
            {
                var post = await handler.GetPostAsync(slug);
                return Results.Ok(post);
            })
            .WithName("GetPostBySlug")
            .WithTags("Posts")
            .Produces<Post>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }

        public static void MapGetAllPostsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/all", async (IGetPostHandler handler, int pageNumber = 1, int pageSize = 10) =>
            {
                var posts = await handler.GetAllPostsAsync(pageNumber, pageSize);
                return Results.Ok(posts);
            })
            .WithName("GetAllPosts")
            .WithTags("Posts")
            .Produces<Post[]>(StatusCodes.Status200OK);
        }
    }
}
