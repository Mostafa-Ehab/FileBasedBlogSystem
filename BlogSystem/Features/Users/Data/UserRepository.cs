using BlogSystem.Domain.Entities;
using BlogSystem.Features.Posts.Data;
using System.Text.Json;

namespace BlogSystem.Features.Users.Data;

public class UserRepository : IUserRepository
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly UserResolver _userResolver;
    private readonly IPostRepository _postRepository;

    public UserRepository(JsonSerializerOptions jsonSerializerOptions, UserResolver userResolver, IPostRepository postRepository)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
        _userResolver = userResolver;
        _postRepository = postRepository;
    }

    public User? GetUserById(string id)
    {
        var path = Path.Combine("Content", "users", id, "profile.json");
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

        var path = Path.Combine("Content", "users", id, "profile.json");
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

        var path = Path.Combine("Content", "users", id, "profile.json");
        if (!File.Exists(path))
        {
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<User>(json, _jsonSerializerOptions);
    }

    public List<User> GetAllUsers()
    {
        var users = new List<User>();
        var path = Path.Combine("Content", "users");
        if (Directory.Exists(path))
        {
            foreach (var id in Directory.GetDirectories(path))
            {
                string json = File.ReadAllText(
                    Path.Combine(id, "profile.json")
                    );
                var user = JsonSerializer.Deserialize<User>(json, _jsonSerializerOptions);
                users.Add(user!);
            }
        }
        return users;
    }

    public User CreateUser(User user)
    {
        var path = Path.Combine("Content", "users", user.Id, "profile.json");
        if (File.Exists(path))
        {
            throw new InvalidOperationException("User already exists");
        }
        Directory.CreateDirectory(Path.Combine("Content", "users", user.Id));
        string json = JsonSerializer.Serialize(user, _jsonSerializerOptions);
        File.WriteAllText(path, json);

        // Update the user resolver cache
        _userResolver.AddUser(user);
        return user;
    }

    public User UpdateUser(User user)
    {
        var path = Path.Combine("Content", "users", user.Id, "profile.json");
        if (!File.Exists(path))
        {
            throw new InvalidOperationException("User does not exist");
        }

        string json = JsonSerializer.Serialize(user, _jsonSerializerOptions);
        File.WriteAllText(path, json);

        _userResolver.UpdateUser(user);
        return user;
    }

    public void DeleteUser(User user)
    {
        var existingUser = GetUserById(user.Id)!;
        var path = Path.Combine("Content", "users", user.Id);

        foreach (var postId in existingUser.Posts)
        {
            var post = _postRepository.GetPostById(postId);
            if (post != null && post.AuthorId == user.Id)
            {
                _postRepository.DeletePost(post);
            }
            else if (post != null)
            {
                post.Editors.Remove(user.Id);
                _postRepository.UpdatePost(post);
            }
        }

        Directory.Delete(path, true);
    }

    public bool UserExists(string id)
    {
        var path = Path.Combine("Content", "users", id, "profile.json");
        return File.Exists(path);
    }

    public bool UserExistsByUsername(string username)
    {
        var id = _userResolver.ResolveUsername(username);
        return !string.IsNullOrWhiteSpace(id) && UserExists(id);
    }

    public bool UserExistsByEmail(string email)
    {
        var id = _userResolver.ResolveEmail(email);
        return !string.IsNullOrWhiteSpace(id) && UserExists(id);
    }
}
