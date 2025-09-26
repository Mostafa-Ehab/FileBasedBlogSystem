

using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using BlogSystem.Features.Users.CreateUser.DTOs;
using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.Login.DTOs;
using BlogSystem.Infrastructure.EmailService;
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
    private readonly IEmailService _emailService;

    public CreateUserHandler(
        IUserRepository userRepository,
        AuthHelper authHelper,
        IValidator<CreateUserRequestDTO> createUserRequestValidator,
        IValidator<RegisterUserRequestDTO> registerUserRequestValidator,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _authHelper = authHelper;
        _createUserRequestValidator = createUserRequestValidator;
        _registerUserRequestValidator = registerUserRequestValidator;
        _emailService = emailService;
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

    public async Task RegisterUserAsync(RegisterUserRequestDTO requestDTO)
    {
        ValidationHelper.Validate(requestDTO, _registerUserRequestValidator);

        var username = SlugHelper.GenerateUniqueSlug(
            requestDTO.Email.Split('@')[0],
            _userRepository.UserExistsByUsername
        );

        // Check if the user already exists by email
        var existingUser = _userRepository.GetUserByEmail(requestDTO.Email);
        var verificationCode =

        if (existingUser == null)
        {
            // Create a new user with the provided details
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
                UpdatedAt = DateTime.UtcNow,
                IsEmailVerified = false // Initially set to false
            };
            _userRepository.CreateUser(user);
        }
        else if (existingUser != null && !existingUser.IsEmailVerified)
        {
            existingUser.Id = username;
            existingUser.Username = username;
            existingUser.Email = requestDTO.Email;
            existingUser.FullName = requestDTO.FullName;
            existingUser.HashedPassword = _authHelper.HashPassword(requestDTO.Password);
            existingUser.Role = UserRole.Viewer;
            existingUser.Bio = requestDTO.Bio ?? string.Empty;
            existingUser.ProfilePictureUrl = ImageHelper.GetRandomProfilePictureUrl();
            existingUser.CreatedAt = DateTime.UtcNow;
            existingUser.UpdatedAt = DateTime.UtcNow;
            existingUser.IsEmailVerified = false;

            _userRepository.UpdateUser(existingUser);
        }
        else
        {
            return;
        }

        _emailService.SendEmailAsync(
            requestDTO.Email,
            EmailRegistrationTemplate.GetEmailSubject(),
            EmailRegistrationTemplate.GetEmailBody(fullName, )
        ).Wait();
    }
}