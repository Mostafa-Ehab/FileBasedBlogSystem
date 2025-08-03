using BlogSystem.Features.Posts.GetPost.DTOs;
using BlogSystem.Features.Users.GetUser.DTOs;

namespace BlogSystem.Features.Users.GetUser;

public interface IGetUserHandler
{
    public Task<GetUserDTO[]> GetAllUsersAsync();
    public Task<GetMyProfileDTO> GetMyProfileAsync(string userId);
    public Task<GetUserDTO> GetUserAsync(string userId);
    public Task<PublicPostDTO[]> GetPublicPostsByUserAsync(string username, int page = 1, int pageSize = 10);
}
