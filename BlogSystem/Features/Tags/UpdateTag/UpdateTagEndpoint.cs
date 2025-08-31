using BlogSystem.Domain.Entities;
using BlogSystem.Features.Tags.UpdateTag.DTOs;

namespace BlogSystem.Features.Tags.UpdateTag;

public static class UpdateTagEndpoint
{
    public static void MapUpdateTagEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/{slug}", async (string slug, UpdateTagRequestDTO request, IUpdateTagHandler handler) =>
        {
            var updatedTag = await handler.UpdateTagAsync(request, slug);
            return Results.Ok(updatedTag);
        })
        .WithName("UpdateTag")
        .RequireAuthorization("Author")
        .Produces<Tag>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Tags");
    }
}
