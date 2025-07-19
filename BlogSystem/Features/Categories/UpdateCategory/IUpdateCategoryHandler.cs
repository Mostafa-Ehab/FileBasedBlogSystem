using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.UpdateCategory.DTOs;

namespace BlogSystem.Features.Categories.UpdateCategory;

public interface IUpdateCategoryHandler
{
    Task<Category> UpdateCategoryAsync(UpdateCategoryRequestDTO request, string slug);
}
