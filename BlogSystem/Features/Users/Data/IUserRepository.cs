using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Users.Data
{
    public interface IUserRepository
    {
        User? GetUserById(string id);
        User? GetUserByUsername(string username);
        User? GetUserByEmail(string email);
        User CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        bool UserExists(string id);
        bool UserExistsByUsername(string username);
        bool UserExistsByEmail(string email);
    }
}
