using Microsoft.Extensions.DependencyInjection;

namespace BlogSystemTest.Users.Data;

public class GetUserByUsernameTest : UnitTestBase
{
    public GetUserByUsernameTest()
    {
        SeedContent();
    }

    [Fact]
    public void GetUserByUsername_ExistingUsername_ReturnsUser()
    {
        var userRepository = CreateUserRepository();
        // Arrange
        var user = userRepository.GetUserByUsername("jane-doe")!;

        // Assert
        Assert.NotNull(user);
        Assert.Equal("jane-doe", user.Id);
        Assert.Equal("jane-doe", user.Username);
        Assert.Equal("jane.doe@example.com", user.Email);
        Assert.Equal("Jane Doe", user.FullName);
        Assert.Equal("This is Jane's profile. They love blogging about technology and travel.", user.Bio);
        Assert.Equal("https://example.com/images/jane-doe.jpg", user.ProfilePictureUrl);
        Assert.Equal(["2025-06-09-first-post"], user.Posts);
    }

    [Fact]
    public void GetUserByUsername_NonExistingUsername_ReturnsNull()
    {
        // Arrange
        var userRepository = CreateUserRepository();

        // Act
        var user = userRepository.GetUserByUsername("non-existing-user");

        // Assert
        Assert.Null(user);
    }
}
