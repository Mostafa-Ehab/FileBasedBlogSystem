using BlogSystem.Domain.Enums;
using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.UpdateUser.DTOs;
using BlogSystem.Infrastructure.ImageService;
using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Exceptions.Users;
using BlogSystem.Shared.Helpers;
using BlogSystem.Shared.Mappings;
using FluentValidation;

namespace BlogSystem.Features.Users.UpdateUser;

public class UpdateUserHandler : IUpdateUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly AuthHelper _authHelper;
    private readonly IValidator<UpdateUserRequestDTO> _updateUserRequestValidator;
    private readonly IValidator<UpdateProfileInfoRequestDTO> _updateProfileInfoRequestValidator;
    private readonly UserImageProvider _imageProvider;

    public UpdateUserHandler(
        IUserRepository userRepository,
        AuthHelper authHelper,
        IValidator<UpdateUserRequestDTO> updateUserRequestValidator,
        IValidator<UpdateProfileInfoRequestDTO> updateProfileInfoRequestValidator,
        UserImageProvider imageProvider
    )
    {
        _userRepository = userRepository;
        _authHelper = authHelper;
        _updateUserRequestValidator = updateUserRequestValidator;
        _updateProfileInfoRequestValidator = updateProfileInfoRequestValidator;
        _imageProvider = imageProvider;
    }

    public Task<UpdatedUserDTO> UpdateUserAsync(UpdateUserRequestDTO request, string userId)
    {
        ValidationHelper.Validate(request, _updateUserRequestValidator);
        var existingUser = _userRepository.GetUserById(userId) ?? throw new UserNotFoundException(userId);

        if (existingUser.Role == UserRole.Admin)
        {
            throw new NotAuthorizedException("Cannot update admin user.");
        }

        if (existingUser.Email != request.Email && _userRepository.UserExistsByEmail(request.Email))
        {
            throw new EmailAlreadyExistException(request.Email);
        }

        if (existingUser.Username != request.Username && _userRepository.UserExistsByUsername(request.Username))
        {
            throw new UsernameAlreadyExistException(request.Username);
        }

        existingUser.Username = request.Username;
        existingUser.Email = request.Email;
        existingUser.FullName = request.FullName;
        existingUser.HashedPassword = string.IsNullOrWhiteSpace(request.Password) ? existingUser.HashedPassword : _authHelper.HashPassword(request.Password);
        existingUser.Role = request.Role;
        existingUser.Bio = request.Bio ?? string.Empty;
        existingUser.UpdatedAt = DateTime.UtcNow;

        var updatedUser = _userRepository.UpdateUser(existingUser);

        return Task.FromResult(updatedUser.MapToUpdatedUserDTO());
    }

    public Task<UpdatedUserDTO> UpdateProfileAsync(UpdateProfileInfoRequestDTO request, string userId)
    {
        ValidationHelper.Validate(request, _updateProfileInfoRequestValidator);
        var existingUser = _userRepository.GetUserById(userId) ?? throw new UserNotFoundException(userId);

        if (existingUser.Email != request.Email && _userRepository.UserExistsByEmail(request.Email))
        {
            throw new EmailAlreadyExistException(request.Email);
        }

        if (existingUser.Username != request.Username && _userRepository.UserExistsByUsername(request.Username))
        {
            throw new UsernameAlreadyExistException(request.Username);
        }

        existingUser.Username = request.Username;
        existingUser.Email = request.Email;
        existingUser.FullName = request.FullName;
        existingUser.Bio = request.Bio ?? string.Empty;
        existingUser.UpdatedAt = DateTime.UtcNow;

        var updatedUser = _userRepository.UpdateUser(existingUser);

        return Task.FromResult(updatedUser.MapToUpdatedUserDTO());
    }

    public async Task<UpdatedUserDTO> ChangeProfilePictureAsync(UpdateProfilePictureRequestDTO request, string userId)
    {
        var existingUser = _userRepository.GetUserById(userId) ?? throw new UserNotFoundException(userId);

        if (request.ProfilePicture == null || request.ProfilePicture.Length == 0)
        {
            throw new ValidationErrorException("Profile picture cannot be empty.");
        }

        existingUser.ProfilePictureUrl = await SaveProfileImage(request.ProfilePicture, userId);
        existingUser.UpdatedAt = DateTime.UtcNow;

        var updatedUser = _userRepository.UpdateUser(existingUser);
        return updatedUser.MapToUpdatedUserDTO();
    }

    public Task<UpdatedUserDTO> ChangePasswordAsync(ChangePasswordRequestDTO request, string userId)
    {
        var existingUser = _userRepository.GetUserById(userId) ?? throw new UserNotFoundException(userId);
        if (!_authHelper.ValidatePassword(request.CurrentPassword, existingUser.HashedPassword))
        {
            throw new ValidationErrorException("Old password is incorrect.");
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            throw new ValidationErrorException("New password must be provided.");
        }

        existingUser.HashedPassword = _authHelper.HashPassword(request.NewPassword);
        existingUser.UpdatedAt = DateTime.UtcNow;

        var updatedUser = _userRepository.UpdateUser(existingUser);
        return Task.FromResult(updatedUser.MapToUpdatedUserDTO());
    }

    private async Task<string> SaveProfileImage(IFormFile image, string userId)
    {
        if (ImageHelper.IsValidImage(image) == false)
        {
            throw new ValidationErrorException("Invalid image format. Only JPEG, PNG, and GIF are allowed.");
        }

        return await _imageProvider.SaveImageAsync(image, userId);
    }
}
