using System.Security.Claims;
using BlogSystem.Features.Posts.PostManagement.DTOs;

namespace BlogSystem.Features.Posts.PostManagement;

public interface IPostManagementHandler
{
    Task<PostResponseDTO> CreatePostAsync(CreatePostRequestDTO request, ClaimsPrincipal user);
    // Task<PostResponseDTO> UpdatePostAsync(UpdatePostRequestDTO request, ClaimsPrincipal user);
    // Task DeletePostAsync(string postId, ClaimsPrincipal user);
}
