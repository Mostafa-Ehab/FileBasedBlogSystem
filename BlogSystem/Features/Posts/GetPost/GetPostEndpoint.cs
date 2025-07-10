using System.Security.Claims;
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

        public static void MapGetPublicPostsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/public", async (IGetPostHandler handler) =>
            {
                var posts = await handler.GetPublicPostsAsync();
                return Results.Ok(posts);
            })
            .WithName("GetAllPosts")
            .WithTags("Posts")
            .Produces<Post[]>(StatusCodes.Status200OK);
        }

        public static void MapSearchPostsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/search", async (IGetPostHandler handler, string query) =>
            {
                var posts = await handler.SearchPostsAsync(query);
                return Results.Ok(posts);
            })
            .WithName("SearchPosts")
            .WithTags("Posts")
            .Produces<Post[]>(StatusCodes.Status200OK);
        }

        public static void MapGetEditorPostsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/editor", async (IGetPostHandler handler) =>
            {
                var posts = await handler.GetEditorPostsAsync();
                return Results.Ok(posts);
            })
            .RequireAuthorization("Editor")
            .WithName("GetEditorPosts")
            .WithTags("Posts")
            .Produces<Post[]>(StatusCodes.Status200OK);
        }

        public static void MapGetAuthorPostsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/author", async (IGetPostHandler handler, ClaimsPrincipal user) =>
            {
                var authorId = user.FindFirstValue("Id")!;
                var posts = await handler.GetAuthorPostsAsync(authorId);
                return Results.Ok(posts);
            })
            .RequireAuthorization("Author")
            .WithName("GetAuthorPosts")
            .WithTags("Posts")
            .Produces<Post[]>(StatusCodes.Status200OK);
        }
    }
}
