using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using BlogSystem.Features.Posts.CommentManagement.DTOs;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Users.Data;
using BlogSystem.Shared.Exceptions.Comments;
using BlogSystem.Shared.Exceptions.Posts;
using BlogSystem.Shared.Exceptions.Users;
using BlogSystem.Shared.Mappings;

namespace BlogSystem.Features.Posts.CommentManagement;

public class CommentManagementHandler : ICommentManagementHandler
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    public CommentManagementHandler(IPostRepository postRepository, IUserRepository userRepository)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
    }

    public CommentResponseDTO[] GetCommentsByPostId(string postId)
    {
        var post = _postRepository.GetPostById(postId);
        if (post == null || post.Status != PostStatus.Published)
        {
            throw new PostNotFoundException(postId);
        }

        var comments = _postRepository.GetCommentsByPostId(postId);
        return comments.Select(c => c.MapToCommentDTO(_userRepository)).ToArray();
    }

    public CommentResponseDTO AddComment(string postId, string userId, CreateCommentRequestDTO request)
    {
        var post = _postRepository.GetPostById(postId);
        if (post == null || post.Status != PostStatus.Published)
        {
            throw new PostNotFoundException(postId);
        }

        var comment = new Comment()
        {
            Id = Guid.NewGuid().ToString(),
            PostId = postId,
            UserId = userId,
            Text = request.Text,
        };

        return _postRepository.CreateComment(comment).MapToCommentDTO(_userRepository);
    }

    public CommentResponseDTO EditComment(string postId, string commentId, string userId, string newCommentText)
    {
        throw new NotImplementedException();
    }

    public void DeleteComment(string postId, string commentId, string userId)
    {
        throw new NotImplementedException();
    }
}