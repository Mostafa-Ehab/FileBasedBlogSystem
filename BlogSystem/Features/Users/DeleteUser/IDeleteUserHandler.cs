namespace BlogSystem.Features.Users.DeleteUser;

public interface IDeleteUserHandler
{
    Task DeleteUserAsync(string userId);
}