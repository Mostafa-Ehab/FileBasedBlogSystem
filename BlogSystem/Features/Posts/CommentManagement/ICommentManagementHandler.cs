
using BlogSystem.Domain.Entities;
using BlogSystem.Features.Posts.CommentManagement.DTOs;

namespace BlogSystem.Features.Posts.CommentManagement;

public interface ICommentManagementHandler
{
    CommentResponseDTO[] GetCommentsByPostId(string postId);
    CommentResponseDTO AddComment(string postId, string userId, CreateCommentRequestDTO request);
    CommentResponseDTO EditComment(string postId, string commentId, string userId, string newCommentText);
    void DeleteComment(string postId, string commentId, string userId);
}
