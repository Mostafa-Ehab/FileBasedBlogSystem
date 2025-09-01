

using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using BlogSystem.Features.Users.CreateUser.DTOs;
using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.Login.DTOs;
using BlogSystem.Shared.Exceptions.Users;
using BlogSystem.Shared.Helpers;
using BlogSystem.Shared.Mappings;
using FluentValidation;
using System.Security.Claims;

namespace BlogSystem.Features.Users.CreateUser;

public class CreateUserHandler : ICreateUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateUserRequestDTO> _createUserRequestValidator;
    private readonly IValidator<RegisterUserRequestDTO> _registerUserRequestValidator;
    private readonly AuthHelper _authHelper;

    public CreateUserHandler(
        IUserRepository userRepository,
        AuthHelper authHelper,
        IValidator<CreateUserRequestDTO> createUserRequestValidator,
        IValidator<RegisterUserRequestDTO> registerUserRequestValidator)
    {
        _userRepository = userRepository;
        _authHelper = authHelper;
        _createUserRequestValidator = createUserRequestValidator;
        _registerUserRequestValidator = registerUserRequestValidator;
    }

    public Task<CreatedUserDTO> CreateUserAsync(CreateUserRequestDTO requestDTO)
    {
        ValidationHelper.Validate(requestDTO, _createUserRequestValidator);

        if (_userRepository.UserExistsByEmail(requestDTO.Email))
        {
            throw new EmailAlreadyExistException(requestDTO.Email);
        }

        if (!string.IsNullOrWhiteSpace(requestDTO.Username) && _userRepository.UserExistsByUsername(requestDTO.Username))
        {
            throw new UsernameAlreadyExistException(requestDTO.Username);
        }
        else if (string.IsNullOrEmpty(requestDTO.Username))
        {
            requestDTO.Username = SlugHelper.GenerateUniqueSlug(
                requestDTO.Email.Split('@')[0],
                _userRepository.UserExistsByUsername
            );
        }

        var user = new User
        {
            Id = requestDTO.Username,
            Username = requestDTO.Username,
            Email = requestDTO.Email,
            FullName = requestDTO.FullName,
            HashedPassword = _authHelper.HashPassword(requestDTO.Password),
            Role = requestDTO.Role ?? UserRole.Author,
            Bio = requestDTO.Bio ?? string.Empty,
            ProfilePictureUrl = ImageHelper.GetRandomProfilePictureUrl(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var createdUser = _userRepository.CreateUser(user);

        return Task.FromResult(createdUser.MapToCreatedUserDTO());
    }

    public Task<RegisterUserResponseDTO> RegisterUserAsync(RegisterUserRequestDTO requestDTO)
    {
        ValidationHelper.Validate(requestDTO, _registerUserRequestValidator);

        if (_userRepository.UserExistsByEmail(requestDTO.Email))
        {
            throw new EmailAlreadyExistException(requestDTO.Email);
        }

        var username = SlugHelper.GenerateUniqueSlug(
            requestDTO.Email.Split('@')[0],
            _userRepository.UserExistsByUsername
        );

        var user = new User
        {
            Id = username,
            Username = username,
            Email = requestDTO.Email,
            FullName = requestDTO.FullName,
            HashedPassword = _authHelper.HashPassword(requestDTO.Password),
            Role = UserRole.Viewer,
            Bio = requestDTO.Bio ?? string.Empty,
            ProfilePictureUrl = ImageHelper.GetRandomProfilePictureUrl(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var createdUser = _userRepository.CreateUser(user);

        return Task.FromResult(new RegisterUserResponseDTO
        {
            Id = user.Id.ToString(),
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            ProfilePictureUrl = user.ProfilePictureUrl,
            AccessToken = _authHelper.GenerateJWTToken([
                new Claim("Id", user.Id.ToString()),
                new Claim("Role", user.Role.ToString())
            ]),
            Role = user.Role
        });
    }
}