using BlogSystem.Features.Users.UpdateUser.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogSystem.Features.Users.UpdateUser;

public static class UpdateUserEndpoint
{
    public static void MapUpdateUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("/{userId}", async (IUpdateUserHandler handler, [FromBody] UpdateUserRequestDTO request, [FromRoute] string userId) =>
        {
            var updatedUser = await handler.UpdateUserAsync(request, userId);
            return Results.Ok(updatedUser);
        })
        .RequireAuthorization("Admin")
        .WithName("UpdateUser")
        .WithTags("Users")
        .Produces<UpdatedUserDTO>()
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    public static void MapUpdateProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/me/profile-info", async (IUpdateUserHandler handler, [FromBody] UpdateProfileInfoRequestDTO profile, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue("Id")!;
            var updatedProfile = await handler.UpdateProfileAsync(profile, userId);
            return Results.Ok(updatedProfile);
        })
        .RequireAuthorization()
        .WithName("UpdateProfile")
        .WithTags("Users")
        .Produces<UpdatedUserDTO>()
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    public static void MapUpdateProfilePictureEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/me/profile-picture", async (IUpdateUserHandler handler, [FromForm] UpdateProfilePictureRequestDTO request, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue("Id")!;
            var updatedUser = await handler.ChangeProfilePictureAsync(request, userId);
            return Results.Ok(updatedUser);
        })
        .RequireAuthorization()
        .DisableAntiforgery()
        .WithName("UpdateProfilePicture")
        .WithTags("Users")
        .Produces<UpdatedUserDTO>()
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    public static void MapChangePasswordEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/me/change-password", async (IUpdateUserHandler handler, [FromBody] ChangePasswordRequestDTO changePasswordRequest, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue("Id")!;
            var updatedUser = await handler.ChangePasswordAsync(changePasswordRequest, userId);
            return Results.Ok(updatedUser);
        })
        .RequireAuthorization()
        .WithName("ChangePassword")
        .WithTags("Users")
        .Produces<UpdatedUserDTO>()
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
