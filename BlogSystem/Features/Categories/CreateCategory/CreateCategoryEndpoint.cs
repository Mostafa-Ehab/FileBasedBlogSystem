using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.CreateCategory.DTOs;

namespace BlogSystem.Features.Categories.CreateCategory
{
    public static class CreateCategoryEndpoint
    {
        public static void MapCreateCategoryEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/", async (ICreateCategoryHandler handler, CreateCategoryRequestDTO request) =>
            {
                var category = await handler.CreateCategoryAsync(request);
                return Results.Created($"/api/categories/{category.Slug}", category);
            })
            .WithName("CreateCategory")
            .WithTags("Categories")
            .Produces<Category>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization("Admin");
        }
    }
}
