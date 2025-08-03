using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.GetPost.DTOs;
using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.GetUser.DTOs;
using BlogSystem.Shared.Exceptions.Users;
using BlogSystem.Shared.Mappings;

namespace BlogSystem.Features.Users.GetUser;

public class GetUserHandler : IGetUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;

    public GetUserHandler(IUserRepository userRepository, IPostRepository postRepository)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;
    }

    public Task<GetUserDTO[]> GetAllUsersAsync()
    {
        var users = _userRepository.GetAllUsers();
        return Task.FromResult(
            users.Select(
                user => user.MapToGetUserDTO()
            ).ToArray()
        );
    }

    public Task<GetMyProfileDTO> GetMyProfileAsync(string userId)
    {
        var user = _userRepository.GetUserById(userId);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }
        return Task.FromResult(user.MapToGetMyProfileDTO());
    }

    public Task<GetUserDTO> GetUserAsync(string userId)
    {
        var user = _userRepository.GetUserByUsername(userId);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }

        user.Posts = user.Posts.Where(postId =>
        {
            Post? post = _postRepository.GetPostById(postId);
            return post != null && post.Status == PostStatus.Published;
        }).ToArray();

        return Task.FromResult(user.MapToGetUserDTO());
    }

    public Task<PublicPostDTO[]> GetPublicPostsByUserAsync(string username, int page = 1, int pageSize = 10)
    {
        var user = _userRepository.GetUserByUsername(username);
        if (user == null)
        {
            throw new UserNotFoundException(username);
        }

        var posts = _postRepository.GetAuthorPublicPosts(user.Id, page, pageSize)
            .Select(post => post.MapToPublicPostDTO(_userRepository))
            .ToArray();

        return Task.FromResult(posts);
    }
}
