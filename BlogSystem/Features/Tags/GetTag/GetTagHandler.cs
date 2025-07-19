using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.PostManagement.DTOs;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Features.Users.Data;
using BlogSystem.Shared.Exceptions.Tags;
using BlogSystem.Shared.Mappings;

namespace BlogSystem.Features.Tags.GetTag;

public class GetTagHandler : IGetTagHandler
{
    private readonly ITagRepository _tagRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;

    public GetTagHandler(ITagRepository tagRepository, IPostRepository postRepository, IUserRepository userRepository)
    {
        _tagRepository = tagRepository;
        _postRepository = postRepository;
        _userRepository = userRepository;
    }

    public Task<Tag> GetTagAsync(string slug)
    {
        Tag? tag = _tagRepository.GetTagBySlug(slug);
        if (tag == null)
        {
            throw new TagNotFoundException(slug);
        }
        return Task.FromResult(tag);
    }

    public Task<Tag[]> GetAllTagsAsync()
    {
        Tag[] tags = _tagRepository.GetAllTags();
        return Task.FromResult(tags);
    }

    public Task<PostResponseDTO[]> GetPostsByTagAsync(string tagSlug)
    {
        Post[] posts = _postRepository.GetPostsByTag(tagSlug)
                    .Where(post => post.Status == PostStatus.Published)
                    .ToArray();

        if (posts == null || posts.Length == 0)
        {
            throw new TagNotFoundException(tagSlug);
        }

        return Task.FromResult(
            posts.Select(
                post => post.MapToPostResponseDTO(_userRepository)
            ).ToArray()
        );
    }
}
