using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.Login.DTOs;
using BlogSystem.Shared.Exceptions.Users;
using BlogSystem.Shared.Helpers;
using FluentValidation;
using System.Security.Claims;

namespace BlogSystem.Features.Users.Login;

public class LoginHandler : ILoginHandler
{
    private readonly IValidator<LoginRequestDTO> _loginRequestValidator;
    private readonly IUserRepository _userRepository;
    private readonly AuthHelper _authHelper;
    public LoginHandler(IUserRepository userRepository, AuthHelper authHelper, IValidator<LoginRequestDTO> validator)
    {
        _loginRequestValidator = validator;
        _userRepository = userRepository;
        _authHelper = authHelper;
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
}
