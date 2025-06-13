using System.Text.Json;
using BlogSystem.Features.Categories.Data;
using Microsoft.Extensions.DependencyInjection;

namespace BlogSystemTest.Categories.Data
{
    public class GetAllCategoriesTest : UnitTestBase
    {
        [Fact]
        public void GetAllCategories_ShouldReturnCategories_WhenCategoriesExist()
        {
            // Arrange
            var categoryRepository = _scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
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