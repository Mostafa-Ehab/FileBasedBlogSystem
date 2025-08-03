using BlogSystem.Domain.Entities;
using BlogSystem.Features.Posts.PostManagement.DTOs;

namespace BlogSystem.Features.Tags.GetTag;

public interface IGetTagHandler
{
    Task<Tag> GetTagAsync(string slug);
    Task<Tag[]> GetAllTagsAsync();
    Task<PostResponseDTO[]> GetPostsByTagAsync(string tagSlug, int page = 1, int pageSize = 10);
}
