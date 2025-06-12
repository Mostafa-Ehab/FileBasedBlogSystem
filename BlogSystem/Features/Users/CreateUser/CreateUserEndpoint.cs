using BlogSystem.Features.Users.CreateUser.DTOs;

namespace BlogSystem.Features.Users.CreateUser
{
    public static class CreateUserEndpoint
    {
        public static void MapCreateUserEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/", async (CreateUserRequestDTO createUserRequestDTO, ICreateUserHandler createUserHandler) =>
            {
                var response = await createUserHandler.CreateUserAsync(createUserRequestDTO);
                return Results.Ok(response);
            })
            .RequireAuthorization("Admin")
            .WithName("CreateUser")
            .Produces<CreateUserResponseDTO>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithTags("Users");
        }
    }
}