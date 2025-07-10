using AutoMapper;
using BlogSystem.Domain.Entities;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.GetPost.DTOs;
using BlogSystem.Features.Users.Data;
using BlogSystem.Infrastructure.MarkdownService;
using BlogSystem.Shared.Exceptions.Posts;
using BlogSystem.Shared.Mappings;

namespace BlogSystem.Features.Posts.Get
{
    public class GetPostHandler : IGetPostHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly MarkdownService _markdownService;

        public GetPostHandler(IPostRepository postRepository, IUserRepository userRepository, MarkdownService markdownService)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _markdownService = markdownService;
        }

        public Task<PublicPostDTO> GetPostAsync(string postSlug)
        {
            var post = _postRepository.GetPostBySlug(postSlug) ?? throw new PostNotFoundException(postSlug);
            var postDto = post.MapToPublicPostDTO(_userRepository);
            postDto.Content = _markdownService.RenderMarkdown(post.Content ?? string.Empty);
            return Task.FromResult(postDto);
        }

        public Task<PublicPostDTO[]> GetPublicPostsAsync(int pageNumber, int pageSize)
        {
            var posts = _postRepository.GetPublicPosts(pageNumber, pageSize);
            var postsDto = posts.Select(
                p => p.MapToPublicPostDTO(_userRepository)
            ).Select(
                p =>
                {
                    p.Content = _markdownService.RenderMarkdown(p.Content ?? string.Empty);
                    return p;
                }
            ).ToArray();
            return Task.FromResult(postsDto);
        }

        public Task<PublicPostDTO[]> SearchPostsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetPublicPostsAsync(1, 10); // Default to first page with 10 posts
            }

            var posts = _postRepository.GetAllPosts(1, 100)
                .Where(p => p.Content!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       p.Tags.Any(t => t.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                       p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                    );

            return Task.FromResult(
                posts.Select(p => p.MapToPublicPostDTO(_userRepository))
                    .Select(p =>
                    {
                        p.Content = _markdownService.RenderMarkdown(p.Content ?? string.Empty);
                        return p;
                    })
                    .ToArray()
            );
        }

        public Task<ManagedPostDTO[]> GetEditorPostsAsync(int pageNumber, int pageSize)
        {
            var posts = _postRepository.GetAllPosts(pageNumber, pageSize);
            return Task.FromResult(
                posts.Select(p => p.MapToManagedPostDTO(_userRepository)).ToArray()
            );
        }

        public Task<ManagedPostDTO[]> GetAuthorPostsAsync(string authorId, int pageNumber, int pageSize)
        {
            var posts = _postRepository.GetAuthorPosts(authorId, pageNumber, pageSize);
            return Task.FromResult(
                posts.Select(p => p.MapToManagedPostDTO(_userRepository)).ToArray()
            );
        }
    }
}
