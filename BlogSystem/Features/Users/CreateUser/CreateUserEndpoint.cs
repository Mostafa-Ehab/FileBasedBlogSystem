using BlogSystem.Features.Users.CreateUser.DTOs;

namespace BlogSystem.Features.Users.CreateUser;

public static class CreateUserEndpoint
{
    public static void MapCreateUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (CreateUserRequestDTO createUserRequestDTO, ICreateUserHandler createUserHandler) =>
        {
            var response = await createUserHandler.CreateUserAsync(createUserRequestDTO);
            return Results.Ok(response);
        })
        .RequireAuthorization("Admin")
        .WithName("CreateUser")
        .Produces<CreatedUserDTO>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status409Conflict)
        .WithTags("Users");

        app.MapPost("/register", async (RegisterUserRequestDTO registerUserRequestDTO, ICreateUserHandler createUserHandler) =>
        {
            var response = await createUserHandler.RegisterUserAsync(registerUserRequestDTO);
            return Results.Ok(response);
        })
        .WithTags("Users")
        .Produces<CreatedUserDTO>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}