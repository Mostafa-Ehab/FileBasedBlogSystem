

using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using BlogSystem.Features.Users.CreateUser.DTOs;
using BlogSystem.Features.Users.Data;
using BlogSystem.Shared.Exceptions.Users;
using BlogSystem.Shared.Helpers;
using FluentValidation;

namespace BlogSystem.Features.Users.CreateUser
{
    public class CreateUserHandler : ICreateUserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<CreateUserRequestDTO> _createUserRequestValidator;
        private readonly AuthHelper _authHelper;

        public CreateUserHandler(IUserRepository userRepository, AuthHelper authHelper, IValidator<CreateUserRequestDTO> createUserRequestValidator)
        {
            _userRepository = userRepository;
            _authHelper = authHelper;
            _createUserRequestValidator = createUserRequestValidator;
        }

        public Task<CreateUserResponseDTO> CreateUserAsync(CreateUserRequestDTO requestDTO)
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
                requestDTO.Username = SlugHelper.GenerateUniqueSlug(requestDTO.Email.Split('@')[0], _userRepository.UserExistsByUsername);
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
                ProfilePictureUrl = string.IsNullOrWhiteSpace(requestDTO.ProfilePictureUrl) ?
                                ImageHelper.GetRandomProfilePictureUrl() : requestDTO.ProfilePictureUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var createdUser = _userRepository.CreateUser(user);

            return Task.FromResult(new CreateUserResponseDTO
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                FullName = createdUser.FullName,
                Bio = createdUser.Bio,
                Role = createdUser.Role,
                ProfilePictureUrl = createdUser.ProfilePictureUrl,
                CreatedAt = createdUser.CreatedAt,
            });
        }
    }
}