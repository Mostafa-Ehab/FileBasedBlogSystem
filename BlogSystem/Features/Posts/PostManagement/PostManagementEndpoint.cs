using System.Security.Claims;
using BlogSystem.Features.Posts.PostManagement.DTOs;
using Microsoft.AspNetCore.Mvc;

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
        .WithSummary("Create or update a post based on the provided request.");
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
        .WithSummary("Update an existing post by its slug.");
    }
}
