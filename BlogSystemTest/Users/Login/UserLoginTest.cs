using System.Text.Json;
using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.Login;
using BlogSystem.Features.Users.Login.DTOs;
using BlogSystem.Shared.Exceptions.Users;
using BlogSystem.Shared.Helpers;
using Microsoft.Extensions.Configuration;

namespace BlogSystemTest.Users.Login
{
    public class UserLoginTest
    {
        private readonly LoginHandler _loginHandler;

        public UserLoginTest()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<UserLoginTest>();
            var configuration = builder.Build();

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var userRepository = new UserRepository(jsonSerializerOptions);
            var authHelper = new AuthHelper(configuration);
            _loginHandler = new LoginHandler(userRepository, authHelper);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "jane-doe",
                Password = "password123"
            };

            // Act
            var response = await _loginHandler.LoginAsync(loginRequest);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response.Token);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ThrowsNotAuthenticatedException()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "jane-doe",
                Password = "wrongpassword"
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotAuthenticatedException>(() => _loginHandler.LoginAsync(loginRequest));
        }
    }
}