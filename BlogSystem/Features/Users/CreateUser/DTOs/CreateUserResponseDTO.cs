using BlogSystem.Domain.Enums;

namespace BlogSystem.Features.Users.CreateUser.DTOs
{
    public class CreateUserResponseDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Author; // Default role
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to current time
    }
}