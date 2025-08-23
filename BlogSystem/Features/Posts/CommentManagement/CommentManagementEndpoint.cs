using BlogSystem.Features.Posts.CommentManagement.DTOs;
using System.Security.Claims;

namespace BlogSystem.Features.Posts.CommentManagement;

public static class CommentManagementEndpoint
{
    public static void MapCommentManagementEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{postId}/comments", (string postId, ICommentManagementHandler handler) =>
        {
            return handler.GetCommentsByPostId(postId);
        })
        .WithTags("Comments")
        .WithSummary("Get all comments for a specific post.");

        app.MapPost("/{postId}/comments", (string postId, CreateCommentRequestDTO request, ClaimsPrincipal user, ICommentManagementHandler handler) =>
        {
            var userId = user.FindFirstValue("Id")!;
            return handler.AddComment(postId, userId, request);
        })
        .RequireAuthorization()
        .WithTags("Comments")
        .WithSummary("Add a comment to a specific post.");
    }
}