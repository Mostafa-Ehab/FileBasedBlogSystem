using BlogSystem.Features.Posts.GetPost.DTOs;
using BlogSystem.Features.Users.GetUser.DTOs;
using System.Security.Claims;

namespace BlogSystem.Features.Users.GetUser;

public static class GetUserEndpoint
{
    public static void MapGetUsersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (IGetUserHandler handler) =>
        {
            var response = await handler.GetAllUsersAsync();
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithName("GetAllUsers")
        .WithTags("Users")
        .Produces<GetUserDTO[]>(StatusCodes.Status200OK);
    }

    public static void MapGetMyProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/me", async (IGetUserHandler handler, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue("Id")!;
            var response = await handler.GetMyProfileAsync(userId);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithName("GetMyProfile")
        .WithTags("Users")
        .Produces<GetMyProfileDTO>(StatusCodes.Status200OK);
    }

    public static void MapGetUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{username}", async (IGetUserHandler handler, ClaimsPrincipal user, string username) =>
        {
            var response = await handler.GetUserAsync(username);
            return Results.Ok(response);
        })
        .WithName("GetUser")
        .WithTags("Users")
        .Produces<GetUserDTO>(StatusCodes.Status200OK);
    }

    public static void MapGetPublicPostsByUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{username}/posts", async (IGetUserHandler handler, string username, int page = 1, int pageSize = 10) =>
        {
            var response = await handler.GetPublicPostsByUserAsync(username, page, pageSize);
            return Results.Ok(response);
        })
        .WithName("GetPublicPostsByUser")
        .WithTags("Users")
        .Produces<PublicPostDTO[]>(StatusCodes.Status200OK);
    }

}
