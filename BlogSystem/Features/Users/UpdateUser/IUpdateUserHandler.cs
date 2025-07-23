using BlogSystem.Features.Users.UpdateUser.DTOs;

namespace BlogSystem.Features.Users.UpdateUser;

public interface IUpdateUserHandler
{
    Task<UpdatedUserDTO> UpdateUserAsync(UpdateUserRequestDTO request, string userId);
    Task<UpdatedUserDTO> UpdateProfileAsync(UpdateProfileInfoRequestDTO profile, string userId);
    Task<UpdatedUserDTO> ChangeProfilePictureAsync(UpdateProfilePictureRequestDTO request, string userId);
    Task<UpdatedUserDTO> ChangePasswordAsync(ChangePasswordRequestDTO changePasswordRequest, string userId);
}