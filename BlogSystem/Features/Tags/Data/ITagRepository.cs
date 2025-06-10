using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Tags.Data
{
    public interface ITagRepository
    {
        Tag? GetTagBySlug(string slug);
        Tag[] GetAllTags();
    }
}