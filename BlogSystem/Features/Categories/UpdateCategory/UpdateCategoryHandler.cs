using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Categories.UpdateCategory.DTOs;
using BlogSystem.Shared.Exceptions.Categories;

namespace BlogSystem.Features.Categories.UpdateCategory;

public class UpdateCategoryHandler : IUpdateCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Category> UpdateCategoryAsync(UpdateCategoryRequestDTO request, string slug)
    {
        var existingCategory = _categoryRepository.GetCategoryBySlug(slug);
        if (existingCategory == null)
        {
            throw new CategoryNotFoundException(slug);
        }

        existingCategory.Description = request.Description;

        return await Task.FromResult(
            _categoryRepository.UpdateCategory(existingCategory)
        );
    }
}
