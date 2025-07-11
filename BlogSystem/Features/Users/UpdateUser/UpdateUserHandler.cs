using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.UpdateUser.DTOs;
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

    public UpdateUserHandler(
        IUserRepository userRepository,
        AuthHelper authHelper,
        IValidator<UpdateUserRequestDTO> updateUserRequestValidator,
        IValidator<UpdateProfileInfoRequestDTO> updateProfileInfoRequestValidator
    )
    {
        _userRepository = userRepository;
        _authHelper = authHelper;
        _updateUserRequestValidator = updateUserRequestValidator;
        _updateProfileInfoRequestValidator = updateProfileInfoRequestValidator;
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

    public Task<UpdatedUserDTO> UpdateProfileAsync(UpdateProfileInfoRequestDTO profile, string userId)
    {
        // Implementation for updating user profile
        throw new NotImplementedException();
    }

    public Task<UpdatedUserDTO> ChangePasswordAsync(ChangePasswordRequestDTO changePasswordRequest, string userId)
    {
        // Implementation for changing user password
        throw new NotImplementedException();
    }
}
