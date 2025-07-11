using BlogSystem.Domain.Enums;

namespace BlogSystem.Features.Users.UpdateUser.DTOs
{
    public class UpdateUserRequestDTO
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}