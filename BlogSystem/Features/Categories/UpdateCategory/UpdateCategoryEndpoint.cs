using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.UpdateCategory.DTOs;

namespace BlogSystem.Features.Categories.UpdateCategory;

public static class UpdateCategoryEndpoint
{
    public static void MapUpdateCategoryEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/{slug}", async (string slug, UpdateCategoryRequestDTO request, IUpdateCategoryHandler handler) =>
        {
            var updatedCategory = await handler.UpdateCategoryAsync(request, slug);
            return Results.Ok(updatedCategory);
        })
        .WithName("UpdateCategory")
        .RequireAuthorization("Admin")
        .Produces<Category>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Categories");
    }
}