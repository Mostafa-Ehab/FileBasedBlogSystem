using System.Security.Claims;
using BlogSystem.Features.Posts.CreatePost;
using BlogSystem.Features.Posts.CreatePost.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BlogSystem.Features.Posts.Create
{
    public static class CreatePostEnpoint
    {
        public static void MapCreatePostEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("", async (ICreatePostHandler handler, ClaimsPrincipal user, [FromForm] CreatePostRequestDTO request) =>
            {
                var response = await handler.CreatePostAsync(request, user);
                return Results.Created($"/api/posts/{response.Slug}", response);
            })
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithName("CreatePost")
            .WithTags("Posts");
        }
    }
}
