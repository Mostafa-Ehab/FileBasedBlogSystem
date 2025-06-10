using BlogSystem.Domain.Entities;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Shared.Exceptions.Posts;

namespace BlogSystem.Features.Posts.Get
{
    public class GetPostHandler : IGetPostHandler
    {
        private readonly IPostRepository _postRepository;
        
        public GetPostHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        
        public Task<Post> GetPostAsync(string postSlug)
        {
            var post = _postRepository.GetPostBySlug(postSlug);
            if (post == null)
            {
                throw new PostNotFoundException(postSlug);
            }
            return Task.FromResult(post);
        }
    }
}
