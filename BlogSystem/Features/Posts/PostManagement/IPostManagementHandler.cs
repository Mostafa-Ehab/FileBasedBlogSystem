using BlogSystem.Features.Posts.GetPost.DTOs;
using BlogSystem.Features.Posts.PostManagement.DTOs;

namespace BlogSystem.Features.Posts.PostManagement;

public interface IPostManagementHandler
{
    Task<PostResponseDTO> CreatePostAsync(CreatePostRequestDTO request, string userId);
    Task<PostResponseDTO> UpdatePostAsync(string postId, UpdatePostRequestDTO request, string userId);
    Task DeletePostAsync(string postId, string userId);
    Task<PostAuthorDTO> AddEditorToPostAsync(string postId, string editorId, string userId);
    Task RemoveEditorFromPostAsync(string postId, string editorId, string userId);
    Task<UploadPostContentImageDTO> UploadImageAsync(IFormFile file, string userId);
}
