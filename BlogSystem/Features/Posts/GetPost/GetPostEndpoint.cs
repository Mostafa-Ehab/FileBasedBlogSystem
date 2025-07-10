using System.Security.Claims;
using BlogSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

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

        public static void MapGetAllPostsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/", async (IGetPostHandler handler, ClaimsPrincipal user, [FromQuery] string query) =>
            {
                var userId = user.FindFirstValue("Id");
                if (string.IsNullOrWhiteSpace(userId))
                {
                    var posts = await handler.GetPublicPostsAsync(query);
                    return Results.Ok(posts);
                }
                else
                {
                    var posts = await handler.GetManagedPostsAsync(userId);
                    return Results.Ok(posts);
                }

            })
            .WithName("GetAllPosts")
            .WithTags("Posts")
            .Produces<Post[]>(StatusCodes.Status200OK);
        }
    }
}
