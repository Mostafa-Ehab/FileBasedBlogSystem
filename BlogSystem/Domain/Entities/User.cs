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
        public string ProfilePicture { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public string Role { get; set; } = string.Empty; // e.g., Admin, Editor, Viewer
        public string[] Posts { get; set; } = []; // List of post IDs authored by the user
    }
}
