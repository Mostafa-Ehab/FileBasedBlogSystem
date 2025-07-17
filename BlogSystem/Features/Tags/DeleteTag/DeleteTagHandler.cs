using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Shared.Exceptions.Tags;

namespace BlogSystem.Features.Tags.DeleteTag;

public class DeleteTagHandler : IDeleteTagHandler
{
    private readonly ITagRepository _tagRepository;

    public DeleteTagHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task DeleteTagAsync(string slug)
    {
        var tag = _tagRepository.GetTagBySlug(slug);
        if (tag == null)
        {
            throw new TagNotFoundException(slug);
        }

        _tagRepository.DeleteTag(tag);
        await Task.CompletedTask;
    }
}