using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.CreateCategory.DTOs;
using BlogSystem.Features.Categories.CreateCategory.Validators;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Helpers;
using FluentValidation;

namespace BlogSystem.Features.Categories.CreateCategory;

public class CreateCategoryHandler : ICreateCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<CreateCategoryRequestDTO> _validator;
    public CreateCategoryHandler(ICategoryRepository categoryRepository, IValidator<CreateCategoryRequestDTO> validator)
    {
        _validator = validator;
        _categoryRepository = categoryRepository;
    }

    public async Task<Category> CreateCategoryAsync(CreateCategoryRequestDTO category)
    {
        ValidationHelper.Validate(category, _validator);

        var slug = SlugHelper.GenerateSlug(category.Name);
        if (_categoryRepository.CategoryExists(slug))
        {
            throw new ValidationErrorException($"Category with slug '{slug}' already exists.");
        }

        return await Task.FromResult(_categoryRepository.CreateCategory(new Category
        {
            Name = category.Name,
            Description = category.Description,
            Slug = slug,
            Posts = [],
        }));
    }
}