using BlogSystem.Features.Users.CreateUser;
using BlogSystem.Features.Users.CreateUser.DTOs;
using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Exceptions.Users;
using Microsoft.Extensions.DependencyInjection;

namespace BlogSystemTest.Users.CreateUser
{
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
    }
}