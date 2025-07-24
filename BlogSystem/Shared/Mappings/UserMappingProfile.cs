using BlogSystem.Domain.Entities;
using BlogSystem.Features.Users.CreateUser.DTOs;
using BlogSystem.Features.Users.GetUser.DTOs;
using BlogSystem.Features.Users.UpdateUser.DTOs;

namespace BlogSystem.Shared.Mappings;

public static class UserMappingProfile
{
    public static GetUserDTO MapToGetUserDTO(this User user)
    {
        return new GetUserDTO
        {
            Id = user.Id,
            FullName = user.FullName,
            Username = user.Username,
            Email = user.Email,
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            Posts = user.Posts,
            SocialLinks = user.SocialLinks?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? []
        };
    }

    public static CreatedUserDTO MapToCreatedUserDTO(this User user)
    {
        return new CreatedUserDTO
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            Posts = user.Posts
        };
    }

    public static UpdatedUserDTO MapToUpdatedUserDTO(this User user)
    {
        return new UpdatedUserDTO
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            Posts = user.Posts
        };
    }

    public static GetMyProfileDTO MapToGetMyProfileDTO(this User user)
    {
        return new GetMyProfileDTO
        {
            Id = user.Id,
            FullName = user.FullName,
            Username = user.Username,
            Email = user.Email,
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            Posts = user.Posts,
            SocialLinks = user.SocialLinks?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? []
        };
    }
}