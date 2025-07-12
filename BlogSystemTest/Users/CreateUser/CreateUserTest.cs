using BlogSystem.Domain.Enums;
using BlogSystem.Features.Users.CreateUser;
using BlogSystem.Features.Users.CreateUser.DTOs;
using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Exceptions.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BlogSystemTest.Users.CreateUser;

public class CreateUserTest : UnitTestBase
{

    [Fact]
    public async Task CreateUser_UserWithExistingEmail_ThrowsValidationException()
    {
        // Arrange
        SeedContent();
        var createUserHandler = CreateCreateUserHandler();
        var createUserRequest = new CreateUserRequestDTO
        {
            Username = "john-doe123",
            Email = "john.doe@example.com",
            FullName = "John Doe",
            Password = "P@ssw0rd"
        };

        // Act & Assert
        await Assert.ThrowsAsync<EmailAlreadyExistException>(() => createUserHandler.CreateUserAsync(createUserRequest));
    }

    [Fact]
    public async Task CreateUser_InvalidEmail_ThrowsValidationException()
    {
        // Arrange
        SeedContent();
        var createUserHandler = CreateCreateUserHandler();
        var createUserRequest = new CreateUserRequestDTO
        {
            Username = "john-doe123",
            Email = "invalid-email",
            FullName = "John Doe",
            Password = "short"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationErrorException>(() => createUserHandler.CreateUserAsync(createUserRequest));
    }

    [Fact]
    public async Task CreateUser_ValidRequest_CreatesUser()
    {
        // Arrange
        SeedContent();
        var createUserHandler = CreateCreateUserHandler();
        var createUserRequest = new CreateUserRequestDTO
        {
            Username = "john-doe123",
            Email = "john.doe@mail.com",
            FullName = "John Doe",
            Password = "P@ssw0rd"
        };

        // Act
        var result = await createUserHandler.CreateUserAsync(createUserRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("john-doe123", result.Username);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal("john.doe@mail.com", result.Email);
    }

    [Fact]
    public async Task CreateUser_InvalidUsername_ThrowsValidationException()
    {
        // Arrange
        SeedContent();
        var createUserHandler = CreateCreateUserHandler();
        var createUserRequest = new CreateUserRequestDTO
        {
            Username = "inv@lid-username!", // Invalid characters
            Email = "valid.email@example.com",
            FullName = "Jane Doe",
            Password = "ValidP@ssw0rd"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationErrorException>(() => createUserHandler.CreateUserAsync(createUserRequest));
    }

    [Fact]
    public async Task CreateUser_UserWithExistingUsername_ThrowsValidationException()
    {
        // Arrange
        SeedContent();
        var createUserHandler = CreateCreateUserHandler();
        var createUserRequest = new CreateUserRequestDTO
        {
            Username = "jane-doe", // Existing username
            Email = "unique.email@example.com",
            FullName = "Jane Doe",
            Password = "ValidP@ssw0rd"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UsernameAlreadyExistException>(() => createUserHandler.CreateUserAsync(createUserRequest));
    }

    [Fact]
    public async Task CreateUser_EmptyFields_ThrowsValidationException()
    {
        // Arrange
        SeedContent();
        var createUserHandler = CreateCreateUserHandler();
        var createUserRequest = new CreateUserRequestDTO
        {
            Username = "",
            Email = "",
            FullName = "",
            Password = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationErrorException>(() => createUserHandler.CreateUserAsync(createUserRequest));
    }

    [Fact]
    public async Task CreateUser_EmptyPassword_ThrowsValidationException()
    {
        // Arrange
        SeedContent();
        var createUserHandler = CreateCreateUserHandler();
        var createUserRequest = new CreateUserRequestDTO
        {
            Username = "unique-username",
            Email = "unique.email2@example.com",
            FullName = "Short Password",
            Password = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationErrorException>(() => createUserHandler.CreateUserAsync(createUserRequest));
    }

    [Fact]
    public async Task CreateUser_ValidEmailAndUsername_CreatesJsonFileWithAuthorRole()
    {
        // Arrange
        SeedContent();
        var createUserHandler = CreateCreateUserHandler();
        var createUserRequest = new CreateUserRequestDTO
        {
            Username = "unique-author",
            Email = "author@example.com",
            FullName = "Author User",
            Password = "StrongP@ssw0rd"
        };

        // Act
        var result = await createUserHandler.CreateUserAsync(createUserRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("unique-author", result.Username);
        Assert.Equal("Author User", result.FullName);
        Assert.Equal("author@example.com", result.Email);

        // Check file exists at expected path
        var expectedPath = Path.Combine("Content", "users", "unique-author", "profile.json");
        Assert.True(File.Exists(expectedPath), $"User file not found at {expectedPath}");

        // Check file content for role
        var json = await File.ReadAllTextAsync(expectedPath);
        Assert.Contains("\"role\": \"author\"", json.ToLower());
    }

    [Fact]
    public async Task CreateUser_EditorRoleInRequest_CreatesJsonFileWithEditorRole()
    {
        // Arrange
        SeedContent();
        var createUserHandler = CreateCreateUserHandler();
        var createUserRequest = new CreateUserRequestDTO
        {
            Username = "unique-editor",
            Email = "editor@example.com",
            FullName = "Editor User",
            Password = "StrongP@ssw0rd",
            Role = UserRole.Editor
        };

        // Act
        var result = await createUserHandler.CreateUserAsync(createUserRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("unique-editor", result.Username);
        Assert.Equal("Editor User", result.FullName);
        Assert.Equal("editor@example.com", result.Email);

        // Check file exists at expected path
        var expectedPath = Path.Combine("Content", "users", "unique-editor", "profile.json");
        Assert.True(File.Exists(expectedPath), $"User file not found at {expectedPath}");

        // Check file content for role
        var json = await File.ReadAllTextAsync(expectedPath);
        Assert.Contains("\"role\": \"editor\"", json.ToLower());
    }

    [Fact]
    public async Task CreateUser_AuthorRoleInRequest_CreatesJsonFileWithAuthorRole()
    {
        // Arrange
        SeedContent();
        var createUserHandler = CreateCreateUserHandler();
        var createUserRequest = new CreateUserRequestDTO
        {
            Username = "unique-author2",
            Email = "author2@example.com",
            FullName = "Second Author",
            Password = "StrongP@ssw0rd",
            Role = UserRole.Author
        };

        // Act
        var result = await createUserHandler.CreateUserAsync(createUserRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("unique-author2", result.Username);
        Assert.Equal("Second Author", result.FullName);
        Assert.Equal("author2@example.com", result.Email);

        // Check file exists at expected path
        var expectedPath = Path.Combine("Content", "users", "unique-author2", "profile.json");
        Assert.True(File.Exists(expectedPath), $"User file not found at {expectedPath}");

        // Check file content for role
        var json = await File.ReadAllTextAsync(expectedPath);
        Assert.Contains("\"role\": \"author\"", json.ToLower());
    }

    [Fact]
    public async Task CreateUser_AdminRoleInRequest_CreatesJsonFileWithAdminRole()
    {
        // Arrange
        SeedContent();
        var createUserHandler = CreateCreateUserHandler();
        var createUserRequest = new CreateUserRequestDTO
        {
            Username = "unique-admin",
            Email = "admin@example.com",
            FullName = "Admin User",
            Password = "StrongP@ssw0rd",
            Role = UserRole.Admin
        };

        // Act
        var result = await createUserHandler.CreateUserAsync(createUserRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("unique-admin", result.Username);
        Assert.Equal("Admin User", result.FullName);
        Assert.Equal("admin@example.com", result.Email);

        // Check file exists at expected path
        var expectedPath = Path.Combine("Content", "users", "unique-admin", "profile.json");
        Assert.True(File.Exists(expectedPath), $"User file not found at {expectedPath}");

        // Check file content for role
        var json = await File.ReadAllTextAsync(expectedPath);
        Assert.Contains("\"role\": \"admin\"", json.ToLower());
    }
}