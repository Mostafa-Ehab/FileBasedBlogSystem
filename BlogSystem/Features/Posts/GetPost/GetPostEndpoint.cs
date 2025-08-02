using BlogSystem.Domain.Entities;
using BlogSystem.Features.Users.GetUser.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogSystem.Features.Posts.GetPost;

public static class GetPostEndpoint
{
    public static void MapGetPostEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{slug}", async (string slug, ClaimsPrincipal user, IGetPostHandler handler) =>
        {
            var userId = user.FindFirstValue("Id");
            if (string.IsNullOrWhiteSpace(userId))
            {
                var post = await handler.GetPostAsync(slug);
                return Results.Ok(post);
            }
            else
            {
                var post = await handler.GetManagedPostAsync(slug, userId);
                return Results.Ok(post);
            }
        })
        .WithName("GetPostBySlug")
        .WithTags("Posts")
        .Produces<Post>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/", async (IGetPostHandler handler, ClaimsPrincipal user, [FromQuery] string? query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        {
            var userId = user.FindFirstValue("Id");
            if (string.IsNullOrWhiteSpace(userId))
            {
                var posts = await handler.GetPublicPostsAsync(query, page, pageSize);
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

        app.MapGet("/{postId}/editors", async (string postId, ClaimsPrincipal user, IGetPostHandler handler) =>
        {
            var userId = user.FindFirstValue("Id")!;
            var editors = await handler.GetPostEditorsAsync(postId, userId);
            return Results.Ok(editors);
        })
        .WithName("GetPostEditors")
        .WithTags("Posts")
        .Produces<GetUserDTO[]>(StatusCodes.Status200OK);
    }
}
