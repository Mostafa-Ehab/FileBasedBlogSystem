using System.Text.Json;
using BlogSystem.Features.Posts.Data;
using Microsoft.Extensions.DependencyInjection;

namespace BlogSystemTest.Posts.Data
{
    public class GetPostsByTagTest : UnitTestBase
    {
        public GetPostsByTagTest()
        {
            SeedContent();
        }

        [Fact]
        public void GetPostsByTag_ShouldReturnPosts_WhenTagExists()
        {
            // Arrange
            var postRepository = CreatePostRepository();
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
            var postRepository = CreatePostRepository();
            var slug = "non-existent-tag";

            // Act
            var posts = postRepository.GetPostsByTag(slug);

            // Assert
            Assert.NotNull(posts);
            Assert.Empty(posts);
        }
    }
}