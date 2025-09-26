using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.Authentication.DTOs;
using BlogSystem.Shared.Exceptions.Users;
using BlogSystem.Shared.Helpers;
using FluentValidation;
using System.Security.Claims;
using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using BlogSystem.Infrastructure.EmailService;

namespace BlogSystem.Features.Users.Authentication;

public class AuthenticationHandler : IAuthenticationHandler
{
    private readonly IValidator<LoginRequestDTO> _loginRequestValidator;
    private readonly IUserRepository _userRepository;
    private readonly AuthHelper _authHelper;
    private readonly IEmailService _emailService;
    private readonly IValidator<RegisterUserRequestDTO> _registerUserRequestValidator;
    public AuthenticationHandler(IUserRepository userRepository, AuthHelper authHelper, IValidator<LoginRequestDTO> validator, IValidator<RegisterUserRequestDTO> registerUserRequestValidator, IEmailService emailService)
    {
        _loginRequestValidator = validator;
        _userRepository = userRepository;
        _authHelper = authHelper;
        _emailService = emailService;
        _registerUserRequestValidator = registerUserRequestValidator;
    }

    public Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
    {
        ValidationHelper.Validate(loginRequestDTO, _loginRequestValidator);
        var user =
            _userRepository.GetUserByUsername(loginRequestDTO.Username) ??
            _userRepository.GetUserByEmail(loginRequestDTO.Username);

        if (user == null || !_authHelper.ValidatePassword(loginRequestDTO.Password, user.HashedPassword))
        {
            throw new NotAuthorizedException("Incorrect username or password", 401);
        }

        return Task.FromResult(new LoginResponseDTO
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

    public async Task<object> RegisterUserAsync(RegisterUserRequestDTO requestDTO)
    {
        ValidationHelper.Validate(requestDTO, _registerUserRequestValidator);

        var existingUser = _userRepository.GetUserByEmail(requestDTO.Email);

        if (existingUser != null && existingUser.IsVerified)
        {
            return Task.FromResult<object>(new
            {
                RedirectUrl = $"/verify-email?email={existingUser.Email}"
            });
        }

        if (existingUser != null && !existingUser.IsVerified)
        {
            _userRepository.DeleteUser(existingUser);
        }

        var username = SlugHelper.GenerateUniqueSlug(
            requestDTO.Email.Split('@')[0],
            _userRepository.UserExistsByUsername
        );
        var verificationCode = Random.Shared.Next(100000, 999999).ToString();

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
            IsVerified = false,
            VerificationCode = verificationCode
        };
        var createdUser = _userRepository.CreateUser(user);
        await _emailService.SendEmailAsync(
            to: user.Email,
            subject: "Verify your email",
            body: VerifyEmailTemplate.GetContent(
                user.Username,
                verificationCode
            )
        );
        Console.WriteLine($"Verification code for {user.Email}: {verificationCode}");
        return Task.FromResult<object>(new
        {
            RedirectUrl = $"/verify-email?email={createdUser.Email}"
        });
    }

    public Task<LoginResponseDTO> VerifyEmailAsync(VerifyEmailRequestDTO requestDTO)
    {
        var user = _userRepository.GetUserByEmail(requestDTO.Email);
        if (user == null || user.VerificationCode != requestDTO.VerificationCode)
        {
            throw new NotAuthorizedException("Invalid verification code", 401);
        }

        user.IsVerified = true;
        user.VerificationCode = string.Empty;
        _userRepository.UpdateUser(user);

        return Task.FromResult(new LoginResponseDTO
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