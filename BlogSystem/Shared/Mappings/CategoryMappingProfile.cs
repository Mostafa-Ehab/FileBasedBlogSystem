using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.CreateCategory.DTOs;
using BlogSystem.Features.Categories.GetCategory.DTOs;

namespace BlogSystem.Shared.Mappings;

public static class CategoryMappingProfile
{
    public static CategoryDTO MapToCategoryDTO(this Category category)
    {
        return new CategoryDTO
        {
            Slug = category.Slug,
            Name = category.Name,
            Description = category.Description,
            Posts = [.. category.Posts]
        };
    }
}
