using BlogSystem.Domain.Entities;
using BlogSystem.Features.Posts.CommentManagement.DTOs;
using BlogSystem.Features.Users.Data;
using BlogSystem.Shared.Helpers;

namespace BlogSystem.Shared.Mappings;

public static class CommentMapping
{
    public static CommentResponseDTO MapToCommentDTO(this Comment comment, IUserRepository userRepository)
    {
        return new CommentResponseDTO
        {
            Id = comment.Id,
            PostId = comment.PostId,
            UserId = comment.UserId,
            User = userRepository.GetUserById(comment.UserId)?.MapToCommentAuthorDTO() ?? new CommentAuthorDTO()
            {
                Id = comment.UserId,
                FullName = "Unknown",
                Email = "Unknown",
                Username = "Unknown",
                ProfilePictureUrl = ImageHelper.GetRandomProfilePictureUrl()
            },
            Text = comment.Text,
            CreatedAt = comment.CreatedAt,
        };
    }

    public static CommentAuthorDTO MapToCommentAuthorDTO(this User user)
    {
        return new CommentAuthorDTO
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Username = user.Username,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }
}