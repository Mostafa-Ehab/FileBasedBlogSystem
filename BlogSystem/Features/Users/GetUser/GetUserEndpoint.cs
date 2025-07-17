using BlogSystem.Features.Users.GetUser.DTOs;
using System.Security.Claims;

namespace BlogSystem.Features.Users.GetUser;

public static class GetUserEndpoint
{
    public static void MapGetUsersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (IGetUserHandler getUserHandler) =>
        {
            var response = await getUserHandler.GetAllUsers();
            return Results.Ok(response);
        })
        .RequireAuthorization("Editor")
        .WithName("GetAllUsers")
        .WithTags("Users")
        .Produces<GetUserDTO[]>(StatusCodes.Status200OK);
    }

    public static void MapGetMyProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/me", async (IGetUserHandler getUserHandler, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue("Id")!;
            var response = await getUserHandler.GetMyProfile(userId);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithName("GetMyProfile")
        .WithTags("Users")
        .Produces<GetMyProfileDTO>(StatusCodes.Status200OK);
    }

    public static void MapGetUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{userId}", async (IGetUserHandler getUserHandler, string userId) =>
        {
            var response = await getUserHandler.GetUser(userId);
            return Results.Ok(response);
        })
        .RequireAuthorization("Editor")
        .WithName("GetUser")
        .WithTags("Users")
        .Produces<GetUserDTO>(StatusCodes.Status200OK);
    }

}
