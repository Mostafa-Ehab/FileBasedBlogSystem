using System.Text.Json;
using BlogSystem.Features.Categories.Data;

namespace BlogSystemTest.Categories.Data
{
    public class GetAllCategoriesTest
    {
        private readonly CategoryRepository categoryRepository;

        public GetAllCategoriesTest()
        {
            categoryRepository = new CategoryRepository(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        [Fact]
        public void GetAllCategories_ShouldReturnCategories_WhenCategoriesExist()
        {
            // Arrange
            var expectedCategory = categoryRepository.GetCategoryBySlug("updates");

            // Act
            var categories = categoryRepository.GetAllCategories();

            // Assert
            Assert.NotNull(categories);
            Assert.Equal(3, categories.Length);
            Assert.Contains(categories, c => c.Name == "Updates");
            Assert.Contains(categories, c => c.Name == "Announcements");
            Assert.Contains(categories, c => c.Name == "Final Thoughts");
        }
    }
}