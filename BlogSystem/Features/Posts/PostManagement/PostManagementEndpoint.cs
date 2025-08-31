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
        .RequireAuthorization("Author")
        .DisableAntiforgery()
        .WithTags("Posts")
        .WithSummary("Create a new post.");

        app.MapPut("/{postId}", async (IPostManagementHandler handler, ClaimsPrincipal user, string postId, [FromForm] UpdatePostRequestDTO request) =>
        {
            var userId = user.FindFirstValue("Id")!;
            var result = await handler.UpdatePostAsync(postId, request, userId);
            return Results.Ok(result);
        })
        .RequireAuthorization("Author")
        .DisableAntiforgery()
        .WithTags("Posts")
        .WithSummary("Update an existing post by its id.");

        app.MapDelete("/{postId}", async (IPostManagementHandler handler, ClaimsPrincipal user, string postId) =>
        {
            var userId = user.FindFirstValue("Id")!;
            await handler.DeletePostAsync(postId, userId);
            return Results.NoContent();
        })
        .RequireAuthorization("Author")
        .WithTags("Posts")
        .WithSummary("Delete a post by its id.");

        app.MapPost("/{postId}/editors/", async (IPostManagementHandler handler, ClaimsPrincipal user, string postId, [FromBody] AddEditorRequestDTO request) =>
        {
            var userId = user.FindFirstValue("Id")!;
            var result = await handler.AddEditorToPostAsync(postId, request.EditorId, userId);
            return Results.Ok(result);
        })
        .RequireAuthorization("Author")
        .WithTags("Posts")
        .WithSummary("Add an editor to a post by its id.");

        app.MapDelete("/{postId}/editors/{editorId}", async (IPostManagementHandler handler, ClaimsPrincipal user, string postId, string editorId) =>
        {
            var userId = user.FindFirstValue("Id")!;
            await handler.RemoveEditorFromPostAsync(postId, editorId, userId);
            return Results.NoContent();
        })
        .RequireAuthorization("Author")
        .WithTags("Posts")
        .WithSummary("Remove an editor from a post by its id.");

        app.MapPost("/upload-image", async (IPostManagementHandler handler, ClaimsPrincipal user, IFormFile file) =>
        {
            var userId = user.FindFirstValue("Id")!;
            var result = await handler.UploadImageAsync(file, userId);
            return Results.Ok(result);
        })
        .RequireAuthorization("Author")
        .DisableAntiforgery()
        .WithTags("Posts")
        .WithSummary("Upload an image for a post.");
    }
}
