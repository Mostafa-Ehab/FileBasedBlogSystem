using BlogSystem.Features.Users.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace BlogSystemTest.Users.Data
{
    public class GetUserByIdTest : UnitTestBase
    {
        public GetUserByIdTest()
        {
            SeedContent();
        }

        [Fact]
        public void GetUserById_ExistingId_ReturnsUser()
        {
            // Arrange
            var userRepository = CreateUserRepository();

            // Act
            var user = userRepository.GetUserById("jane-doe")!;

            // Assert
            Assert.NotNull(user);
            Assert.Equal("jane-doe", user.Id);
            Assert.Equal("jane-doe", user.Username);
            Assert.Equal("jane.doe@example.com", user.Email);
            Assert.Equal("Jane Doe", user.FullName);
            Assert.Equal("This is Jane's profile. They love blogging about technology and travel.", user.Bio);
            Assert.Equal("https://example.com/images/jane-doe.jpg", user.ProfilePicture);
            Assert.Equal(["2025-06-09-first-post"], user.Posts);
        }

        [Fact]
        public void GetUserById_NonExistingId_ReturnsNull()
        {
            // Arrange
            var userRepository = CreateUserRepository();

            // Act
            var user = userRepository.GetUserById("non-existing-user");

            // Assert
            Assert.Null(user);
        }
    }
}
