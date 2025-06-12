using BlogSystem.Features.Users.Data;
using System.Text.Json;

namespace BlogSystemTest.Users.Data
{
    public class GetUserByUsernameTest
    {
        private readonly IUserRepository userRepository;
        public GetUserByUsernameTest()
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            userRepository = new UserRepository(jsonSerializerOptions);
        }

        [Fact]
        public void GetUserByUsername_ExistingUsername_ReturnsUser()
        {
            // Arrange
            var user = userRepository.GetUserByUsername("jane-doe")!;

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
        public void GetUserByUsername_NonExistingUsername_ReturnsNull()
        {
            // Arrange
            var user = userRepository.GetUserByUsername("non-existing-user");

            // Assert
            Assert.Null(user);
        }
    }
}
