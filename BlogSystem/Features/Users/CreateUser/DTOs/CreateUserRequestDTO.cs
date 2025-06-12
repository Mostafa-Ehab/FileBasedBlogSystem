using BlogSystem.Domain.Enums;

namespace BlogSystem.Features.Users.CreateUser.DTOs
{
    public class CreateUserRequestDTO
    {
        public string? Username { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole? Role { get; set; }
    }
}