using BlogSystem.Domain.Entities;
using BlogSystem.Features.Tags.UpdateTag.DTOs;

namespace BlogSystem.Features.Tags.UpdateTag;

public interface IUpdateTagHandler
{
    Task<Tag> UpdateTagAsync(UpdateTagRequestDTO request, string slug);
}
