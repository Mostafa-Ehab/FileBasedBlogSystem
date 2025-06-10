using BlogSystem.Domain.Entities;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Shared.Exceptions.Tags;

namespace BlogSystem.Features.Tags.GetTag
{
    public class GetTagHandler : IGetTagHandler
    {
        private readonly ITagRepository _tagRepository;
        private readonly IPostRepository _postRepository;

        public GetTagHandler(ITagRepository tagRepository, IPostRepository postRepository)
        {
            _tagRepository = tagRepository;
            _postRepository = postRepository;
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

        public Task<Post[]> GetPostsByTagAsync(string tagSlug)
        {
            Post[] posts = _postRepository.GetPostsByTag(tagSlug);
            if (posts == null || posts.Length == 0)
            {
                throw new TagNotFoundException(tagSlug);
            }
            return Task.FromResult(posts);
        }
    }
}
