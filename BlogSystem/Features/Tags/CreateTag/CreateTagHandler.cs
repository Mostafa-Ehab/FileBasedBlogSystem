using BlogSystem.Domain.Entities;
using BlogSystem.Features.Tags.CreateTag.DTOs;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Helpers;
using FluentValidation;

namespace BlogSystem.Features.Tags.CreateTag;

public class CreateTagHandler : ICreateTagHandler
{
    private readonly ITagRepository _tagRepository;
    private readonly IValidator<CreateTagRequestDTO> _validator;

    public CreateTagHandler(ITagRepository tagRepository, IValidator<CreateTagRequestDTO> validator)
    {
        _validator = validator;
        _tagRepository = tagRepository;
    }

    public async Task<Tag> CreateTagAsync(CreateTagRequestDTO tag)
    {
        ValidationHelper.Validate(tag, _validator);

        var slug = SlugHelper.GenerateSlug(tag.Name);
        if (_tagRepository.TagExists(slug))
        {
            throw new ValidationErrorException($"Tag with slug '{slug}' already exists.");
        }

        return await Task.FromResult(_tagRepository.CreateTag(new Tag
        {
            Name = tag.Name,
            Description = tag.Description,
            Slug = slug,
            Posts = []
        }));
    }
}
