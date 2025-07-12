using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.GetPost.DTOs;
using BlogSystem.Features.Users.Data;
using BlogSystem.Infrastructure.MarkdownService;
using BlogSystem.Shared.Exceptions.Posts;
using BlogSystem.Shared.Mappings;

namespace BlogSystem.Features.Posts.GetPost;

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

    public Task<PublicPostDTO[]> GetPublicPostsAsync(string? query)
    {
        var posts = _postRepository.GetPublicPosts();

        if (!string.IsNullOrWhiteSpace(query))
        {
            posts = posts.Where(p => p.Content!.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                   p.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                   p.Tags.Any(t => t.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                   p.Description.Contains(query, StringComparison.OrdinalIgnoreCase)
                ).ToArray();
        }

        return Task.FromResult(
            posts.Select(
                p => p.MapToPublicPostDTO(_userRepository)
            ).Select(
                p =>
                {
                    p.Content = _markdownService.RenderMarkdown(p.Content ?? string.Empty);
                    return p;
                }
            ).ToArray()
        );
    }

    public Task<ManagedPostDTO[]> GetManagedPostsAsync(string userId)
    {
        var user = _userRepository.GetUserById(userId)!;

        var posts = new List<Post>();
        if (user.Role != UserRole.Editor && user.Role != UserRole.Admin)
        {
            posts = user.Posts
                .Select(_postRepository.GetPostById)
                .OrderByDescending(p => p!.UpdatedAt)
                .ToList()!;
        }
        else
        {
            posts = _postRepository.GetAllPosts().ToList();
        }

        return Task.FromResult(
            posts.Select(p => p.MapToManagedPostDTO(_userRepository)).ToArray()
        );
    }
}
