using System.Text.Json;
using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Users.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly UserResolver _userResolver;

        public UserRepository(JsonSerializerOptions jsonSerializerOptions)
        {
            _jsonSerializerOptions = jsonSerializerOptions;
            _userResolver = new UserResolver();
        }

        public User? GetUserById(string id)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Content", "users", id, "profile.json");
            if (!File.Exists(path))
            {
                return null;
            }

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<User>(json, _jsonSerializerOptions);
        }

        public User? GetUserByUsername(string username)
        {
            var id = _userResolver.ResolveUsername(username);
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var path = Path.Combine(AppContext.BaseDirectory, "Content", "users", id, "profile.json");
            if (!File.Exists(path))
            {
                return null;
            }

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<User>(json, _jsonSerializerOptions);
        }

        public User? GetUserByEmail(string email)
        {
            var id = _userResolver.ResolveEmail(email);
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var path = Path.Combine(AppContext.BaseDirectory, "Content", "users", id, "profile.json");
            if (!File.Exists(path))
            {
                return null;
            }

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<User>(json, _jsonSerializerOptions);
        }

        public User CreateUser(User user)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
