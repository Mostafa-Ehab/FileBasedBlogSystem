using BlogSystem.Features.Posts.PostManagement.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogSystem.Features.Posts.PostManagement;

public static class PostManagementEndpoint
{
    public static void MapPostManagementEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (IPostManagementHandler handler, ClaimsPrincipal user, [FromForm] CreatePostRequestDTO request) =>
        {
            var userId = user.FindFirstValue("Id")!;
            var result = await handler.CreatePostAsync(request, userId);
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .DisableAntiforgery()
        .WithTags("Posts")
        .WithSummary("Create a new post.");
    }

    public static void MapUpdatePostEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/{postId}", async (IPostManagementHandler handler, ClaimsPrincipal user, string postId, [FromForm] UpdatePostRequestDTO request) =>
        {
            var userId = user.FindFirstValue("Id")!;
            var result = await handler.UpdatePostAsync(postId, request, userId);
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .DisableAntiforgery()
        .WithTags("Posts")
        .WithSummary("Update an existing post by its id.");
    }

    public static void MapDeletePostEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/{postId}", async (IPostManagementHandler handler, ClaimsPrincipal user, string postId) =>
        {
            var userId = user.FindFirstValue("Id")!;
            await handler.DeletePostAsync(postId, userId);
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("Posts")
        .WithSummary("Delete a post by its id.");
    }
}
