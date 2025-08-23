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

        // app.MapPost("/posts/{postId}/comments", (string postId, string userId, string commentText, ICommentManagementHandler handler) =>
        // {
        //     return handler.AddComment(postId, userId, commentText);
        // });

        // app.MapPut("/posts/{postId}/comments/{commentId}", (string postId, string commentId, string userId, string newCommentText, ICommentManagementHandler handler) =>
        // {
        //     return handler.EditComment(postId, commentId, userId, newCommentText);
        // });

        // app.MapDelete("/posts/{postId}/comments/{commentId}", (string postId, string commentId, string userId, ICommentManagementHandler handler) =>
        // {
        //     handler.DeleteComment(postId, commentId, userId);
        //     return Results.NoContent();
        // });
    }
}