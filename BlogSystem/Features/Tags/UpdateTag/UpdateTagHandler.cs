using BlogSystem.Domain.Entities;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Features.Tags.UpdateTag.DTOs;
using BlogSystem.Shared.Exceptions.Tags;

namespace BlogSystem.Features.Tags.UpdateTag;

public class UpdateTagHandler : IUpdateTagHandler
{
    private readonly ITagRepository _tagRepository;

    public UpdateTagHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Tag> UpdateTagAsync(UpdateTagRequestDTO request, string slug)
    {
        var existingTag = _tagRepository.GetTagBySlug(slug);
        if (existingTag == null)
        {
            throw new TagNotFoundException(slug);
        }

        existingTag.Description = request.Description;

        return await Task.FromResult(
            _tagRepository.UpdateTag(existingTag)
        );
    }
}
