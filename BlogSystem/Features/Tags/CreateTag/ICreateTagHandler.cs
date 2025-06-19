using BlogSystem.Domain.Entities;
using BlogSystem.Features.Tags.CreateTag.DTOs;

namespace BlogSystem.Features.Tags.CreateTag
{
    public interface ICreateTagHandler
    {
        Task<Tag> CreateTagAsync(CreateTagRequestDTO tag);
    }
}
