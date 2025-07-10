using BlogSystem.Features.Users.GetUser.DTOs;

namespace BlogSystem.Features.Users.GetUser;

public static class GetUserEndpoint
{
    public static void MapGetUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (IGetUserHandler getUserHandler) =>
        {
            var response = await getUserHandler.GetAllUsers();
            return Results.Ok(response);
        })
        .WithName("Get All Users")
        .WithTags("Users")
        .Produces<GetUserDTO[]>(StatusCodes.Status200OK);
    }
}
