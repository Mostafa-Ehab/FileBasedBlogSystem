using BlogSystem.Features.Users.Login;
using BlogSystem.Features.Users.Login.DTOs;
using BlogSystem.Shared.Exceptions.Users;
using Microsoft.Extensions.DependencyInjection;

namespace BlogSystemTest.Users.Login
{
    public class UserLoginTest : UnitTestBase
    {
        public UserLoginTest()
        {
            SeedContent();
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginHandler = CreateLoginHandler();
            var loginRequest = new LoginRequestDTO
            {
                Username = "jane-doe",
                Password = "password123"
            };

            // Act
            var response = await loginHandler.LoginAsync(loginRequest);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response.AccessToken);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ThrowsNotAuthenticatedException()
        {
            // Arrange
            var loginHandler = CreateLoginHandler();
            var loginRequest = new LoginRequestDTO
            {
                Username = "jane-doe",
                Password = "wrongpassword"
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotAuthenticatedException>(() => loginHandler.LoginAsync(loginRequest));
        }
    }
}