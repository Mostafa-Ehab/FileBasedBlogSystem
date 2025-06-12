using BlogSystem.Shared.Helpers;
using Microsoft.Extensions.Configuration;

namespace BlogSystemTest.Helpers
{
    public class PasswordHashTest
    {
        private readonly AuthHelper _authHelper;
        public PasswordHashTest()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<PasswordHashTest>();
            var configuration = builder.Build();
            _authHelper = new AuthHelper(configuration);
        }

        [Fact]
        public void TestPasswordHash()
        {
            string password = "password";
            string hashedPassword = _authHelper.HashPassword(password);
            Assert.NotEqual(password, hashedPassword);
            Assert.True(_authHelper.ValidatePassword(password, hashedPassword));
        }

        [Fact]
        public void TestPasswordHash_InvalidPassword_ReturnsFalse()
        {
            string password = "password";
            string hashedPassword = _authHelper.HashPassword(password);
            Assert.False(_authHelper.ValidatePassword("invalid-password", hashedPassword));
        }
    }
}