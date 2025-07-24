using BlogSystem.Features.Posts.GetPost.DTOs;
using BlogSystem.Features.Users.GetUser.DTOs;

namespace BlogSystem.Features.Users.GetUser;

public interface IGetUserHandler
{
    public Task<GetUserDTO[]> GetAllUsers();
    public Task<GetMyProfileDTO> GetMyProfile(string userId);
    public Task<GetUserDTO> GetUser(string userId);
    public Task<PublicPostDTO[]> GetPublicPostsByUserAsync(string username);
}
