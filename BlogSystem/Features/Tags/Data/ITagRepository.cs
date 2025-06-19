using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Tags.Data
{
    public interface ITagRepository
    {
        Tag? GetTagBySlug(string slug);
        Tag[] GetAllTags();
        bool TagExists(string slug);
        Tag CreateTag(Tag tag);
    }
}