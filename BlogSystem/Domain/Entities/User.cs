using BlogSystem.Domain.Enums;

namespace BlogSystem.Domain.Entities
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public UserRole Role { get; set; } = UserRole.Author; // e.g., Admin, Editor, Author
        public string[] Posts { get; set; } = []; // List of post IDs authored by the user
        public Dictionary<string, string> SocialLinks { get; set; } = [];
    }
}
