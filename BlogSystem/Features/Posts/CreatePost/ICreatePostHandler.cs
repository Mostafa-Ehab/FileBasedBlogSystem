using System.Security.Claims;
using BlogSystem.Features.Posts.CreatePost.DTOs;

namespace BlogSystem.Features.Posts.CreatePost
{
    public interface ICreatePostHandler
    {
        Task<CreatePostResponseDTO> CreatePostAsync(CreatePostRequestDTO request, ClaimsPrincipal user);
    }
}