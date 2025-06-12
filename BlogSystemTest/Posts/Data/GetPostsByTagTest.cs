using System.Text.Json;
using BlogSystem.Features.Posts.Data;

namespace BlogSystemTest.Posts.Data
{
    public class GetPostsByTagTest
    {
        private readonly PostRepository postRepository;

        public GetPostsByTagTest()
        {
            postRepository = new PostRepository(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        [Fact]
        public void GetPostsByTag_ShouldReturnPosts_WhenTagExists()
        {
            // Arrange
            var slug = "update";
            var expectedPost = postRepository.GetPostById("2025-06-09-second-post");

            // Act
            var posts = postRepository.GetPostsByTag(slug);

            // Assert
            Assert.NotNull(posts);
            Assert.Single(posts);
            Assert.Equivalent(expectedPost, posts[0]);
        }

        [Fact]
        public void GetPostsByTag_ShouldReturnEmptyArray_WhenTagDoesNotExist()
        {
            // Arrange
            var slug = "non-existent-tag";

            // Act
            var posts = postRepository.GetPostsByTag(slug);

            // Assert
            Assert.NotNull(posts);
            Assert.Empty(posts);
        }
    }
}