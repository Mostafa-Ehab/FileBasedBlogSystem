using System.Security.Claims;
using BlogSystem.Features.Posts.UpdatePost.DTOs;

namespace BlogSystem.Features.Posts.UpdatePost
{
    public interface IUpdatePostHandler
    {
        Task<UpdatePostResponseDTO> HandleUpdatePostAsync(UpdatePostRequestDTO request, string postId, ClaimsPrincipal user);
    }
}