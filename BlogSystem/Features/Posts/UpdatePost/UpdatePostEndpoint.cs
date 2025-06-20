using System.Security.Claims;
using BlogSystem.Features.Posts.UpdatePost.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BlogSystem.Features.Posts.UpdatePost
{
    public static class UpdatePostEndpoint
    {
        public static void MapUpdatePostEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPut("/{postId}", async (string postId, [FromForm] UpdatePostRequestDTO request, ClaimsPrincipal user, IUpdatePostHandler handler) =>
            {
                var response = await handler.HandleUpdatePostAsync(request, postId, user);
                return Results.Ok(response);
            })
            .WithName("UpdatePost")
            .WithTags("Posts")
            .RequireAuthorization()
            .DisableAntiforgery();
        }
    }
}