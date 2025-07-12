using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.CreateCategory.DTOs;

namespace BlogSystem.Features.Categories.CreateCategory;

public interface ICreateCategoryHandler
{
    Task<Category> CreateCategoryAsync(CreateCategoryRequestDTO category);
}