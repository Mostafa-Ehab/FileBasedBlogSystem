using System.Text.Json;
using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Users.Data
{
    public class UserResolver
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private Dictionary<string, string> usernameCache = new();
        private Dictionary<string, string> emailCache = new();

        public UserResolver()
        {
            _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
            LoadUsers();
        }

        public string? ResolveUsername(string username)
        {
            return usernameCache.TryGetValue(username.ToLowerInvariant(), out var cachedId) ? cachedId : null;
        }

        public string? ResolveEmail(string email)
        {
            return emailCache.TryGetValue(email.ToLowerInvariant(), out var cachedId) ? cachedId : null;
        }

        private void LoadUsers()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Content", "users", "users.json");
            if (!File.Exists(path))
            {
                return;
            }

            string json = File.ReadAllText(path);
            var usersData = JsonSerializer.Deserialize<UsersJson>(json, _jsonSerializerOptions)
                ?? throw new InvalidOperationException("Failed to load users.json");
            usernameCache = usersData.Users.ToDictionary(u => u.Username.ToLowerInvariant(), u => u.Id);
            emailCache = usersData.Users.ToDictionary(u => u.Email.ToLowerInvariant(), u => u.Id);
        }
    }

    internal class UsersJson
    {
        public List<UserData> Users { get; set; } = new();
    }
    internal class UserData
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
