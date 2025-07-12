using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Tags.GetTag;

public interface IGetTagHandler
{
    Task<Tag> GetTagAsync(string slug);
    Task<Tag[]> GetAllTagsAsync();
    Task<Post[]> GetPostsByTagAsync(string tagSlug);
}
