using BlogSystem.Domain.Entities;
using BlogSystem.Features.Tags.CreateTag.DTOs;

namespace BlogSystem.Features.Tags.CreateTag;

public static class CreateTagEndpoint
{
    public static void MapCreateTagEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/", async (ICreateTagHandler handler, CreateTagRequestDTO request) =>
        {
            var tag = await handler.CreateTagAsync(request);
            return Results.Created($"/api/tags/{tag.Slug}", tag);
        })
        .WithName("CreateTag")
        .WithTags("Tags")
        .Produces<Tag>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization("Admin");
    }
}
