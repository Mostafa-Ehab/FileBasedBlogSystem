using BlogSystem.Features.Users.Authentication.DTOs;

namespace BlogSystem.Features.Users.Authentication;

public static class AuthenticationEndpoint
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (IAuthenticationHandler authenticationHandler, LoginRequestDTO loginRequestDTO) =>
        {
            var response = await authenticationHandler.LoginAsync(loginRequestDTO);
            return Results.Ok(response);
        })
        .WithName("Login")
        .WithTags("Authentication")
        .Produces<LoginResponseDTO>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        app.MapPost("/register", async (RegisterUserRequestDTO registerUserRequestDTO, IAuthenticationHandler authenticationHandler) =>
        {
            var response = await authenticationHandler.RegisterUserAsync(registerUserRequestDTO);
            return Results.Ok(response);
        })
        .WithTags("Users")
        .WithName("RegisterUser")
        .Produces<object>(StatusCodes.Status200OK)
        .ProducesValidationProblem();

        app.MapPost("/verify-email", async (IAuthenticationHandler authenticationHandler, VerifyEmailRequestDTO verifyEmailRequestDTO) =>
        {
            var response = await authenticationHandler.VerifyEmailAsync(verifyEmailRequestDTO);
            return Results.Ok(response);
        })
        .WithName("VerifyEmail")
        .WithTags("Authentication")
        .Produces<LoginResponseDTO>(StatusCodes.Status200OK)
        .ProducesValidationProblem();
    }
}
