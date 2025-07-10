using System.Security.Claims;
using BlogSystem.Features.Posts.PostManagement.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BlogSystem.Features.Posts.PostManagement;

public static class PostManagementEndpoint
{
    public static void MapPostManagementEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (IPostManagementHandler handler, ClaimsPrincipal user, [FromForm] CreatePostRequestDTO request) =>
        {
            var result = await handler.CreatePostAsync(request, user);
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .DisableAntiforgery()
        .WithTags("Posts")
        .WithSummary("Create or update a post based on the provided request.");
    }
}
