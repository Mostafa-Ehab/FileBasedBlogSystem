using BlogSystem.Features.Users.Data;
using BlogSystem.Shared.Exceptions.Users;
using BlogSystem.Features.Users.Login.DTOs;
using BlogSystem.Shared.Helpers;
using System.Security.Claims;
using FluentValidation;

namespace BlogSystem.Features.Users.Login
{
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
                throw new NotAuthenticatedException("Incorrect username or password", 401);
            }

            return Task.FromResult(new LoginResponseDTO
            {
                AccessToken = _authHelper.GenerateJWTToken([
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Role", user.Role.ToString())
                ]),
            });
        }
    }
}
