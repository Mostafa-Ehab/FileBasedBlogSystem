using BlogSystem.Features.Users.Login.DTOs;

namespace BlogSystem.Features.Users.Login;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (ILoginHandler loginHandler, LoginRequestDTO loginRequestDTO) =>
        {
            var response = await loginHandler.LoginAsync(loginRequestDTO);
            return Results.Ok(response);
        })
        .WithName("Login")
        .WithTags("Users")
        .Produces<LoginResponseDTO>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}