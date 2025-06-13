using System.Text.Json;
using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Users.Data
{
    public class UserResolver
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private Dictionary<string, string> usernameCache = [];
        private Dictionary<string, string> emailCache = [];
        private Dictionary<string, string[]> idCache = [];

        public UserResolver(JsonSerializerOptions jsonSerializerOptions)
        {
            _jsonSerializerOptions = jsonSerializerOptions;
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
            var path = Path.Combine("Content", "users", "users.json");
            if (!File.Exists(path))
            {
                string data = Directory.GetCurrentDirectory();
                Console.WriteLine($"Warning: {path} does not exist. User resolver will not function properly.");
                return;
            }

            string json = File.ReadAllText(path);
            var usersData = JsonSerializer.Deserialize<UsersJson>(json, _jsonSerializerOptions)
                ?? throw new InvalidOperationException("Failed to load users.json");
            usernameCache = usersData.Users.ToDictionary(u => u.Username.ToLowerInvariant(), u => u.Id);
            emailCache = usersData.Users.ToDictionary(u => u.Email.ToLowerInvariant(), u => u.Id);
            idCache = usersData.Users.ToDictionary(u => u.Id, u => new[] { u.Username.ToLowerInvariant(), u.Email.ToLowerInvariant() });
        }

        public void AddUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Id) || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException("User data cannot have null or empty fields.");
            }

            usernameCache[user.Username.ToLowerInvariant()] = user.Id;
            emailCache[user.Email.ToLowerInvariant()] = user.Id;
            idCache[user.Id] = [user.Username.ToLowerInvariant(), user.Email.ToLowerInvariant()];

            SaveUsers();
        }

        private void SaveUsers()
        {
            var usersData = new UsersJson
            {
                Users = idCache.Select(kvp => new UserData
                {
                    Id = kvp.Key,
                    Username = kvp.Value[0],
                    Email = kvp.Value[1]
                }).ToList()
            };

            var json = JsonSerializer.Serialize(usersData, _jsonSerializerOptions);
            var path = Path.Combine("Content", "users", "users.json");
            File.WriteAllText(path, json);
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
