using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.GetPost.DTOs;
using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.GetUser.DTOs;
using BlogSystem.Infrastructure.MarkdownService;
using BlogSystem.Infrastructure.SearchEngineService;
using BlogSystem.Shared.Exceptions.Posts;
using BlogSystem.Shared.Exceptions.Users;
using BlogSystem.Shared.Mappings;

namespace BlogSystem.Features.Posts.GetPost;

public class GetPostHandler : IGetPostHandler
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly MarkdownService _markdownService;
    private readonly ISearchEngineService<Post> _postSearchEngineService;

    public GetPostHandler(IPostRepository postRepository, IUserRepository userRepository, MarkdownService markdownService, ISearchEngineService<Post> postSearchEngineService)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _markdownService = markdownService;
        _postSearchEngineService = postSearchEngineService;
    }

    public Task<PublicPostDTO> GetPostAsync(string postSlug)
    {
        var post = _postRepository.GetPostBySlug(postSlug) ?? throw new PostNotFoundException(postSlug);
        var postDto = post.MapToPublicPostDTO(_userRepository);
        postDto.Content = _markdownService.RenderMarkdown(post.Content ?? string.Empty);
        return Task.FromResult(postDto);
    }

    public Task<ManagedPostDTO> GetManagedPostAsync(string postId, string userId)
    {
        var post = _postRepository.GetPostById(postId) ?? throw new PostNotFoundException(postId);
        var user = _userRepository.GetUserById(userId)!;

        if (user.Role != UserRole.Admin && user.Role != UserRole.Author && user.Id != post.AuthorId)
        {
            throw new NotAuthorizedException("You don't have access to this content");
        }

        return Task.FromResult(
            post.MapToManagedPostDTO(_userRepository)
        );
    }

    public Task<PublicPostDTO[]> GetPublicPostsAsync(string? query)
    {
        var posts = _postRepository.GetPublicPosts();

        if (!string.IsNullOrWhiteSpace(query))
        {
            posts = _postSearchEngineService.SearchDocumentsAsync(query).Result.ToArray();
            posts = posts.Select(p => _postRepository.GetPostById(p.Id))
                        .Where(p => p != null && p.Status == PostStatus.Published)
                        .Select(p => p!)
                        .ToArray();
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
        if (user.Role == UserRole.Admin)
        {
            posts = _postRepository.GetAllPosts()
                .Where(p => p.Status != PostStatus.Draft || p.AuthorId == user.Id || p.Editors.Contains(user.Id))
                .ToList();
        }
        else
        {
            posts = _postRepository.GetAuthorPosts(user.Id).ToList();
        }

        return Task.FromResult(
            posts.Select(p => p.MapToManagedPostDTO(_userRepository)).ToArray()
        );
    }

    public Task<GetUserDTO[]> GetPostEditorsAsync(string postId, string userId)
    {
        var post = _postRepository.GetPostById(postId) ?? throw new PostNotFoundException(postId);
        var user = _userRepository.GetUserById(userId)!;
        if (user.Id != post.AuthorId)
        {
            throw new NotAuthorizedException("You don't have access to this content");
        }

        var editors = post.Editors.Select(_userRepository.GetUserById);

        return Task.FromResult(
            editors.Select(editor => editor!.MapToGetUserDTO()).ToArray()
        );
    }
}
