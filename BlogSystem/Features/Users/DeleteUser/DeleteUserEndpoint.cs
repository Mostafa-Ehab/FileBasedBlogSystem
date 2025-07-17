namespace BlogSystem.Features.Users.DeleteUser;

public static class DeleteUserEndpoint
{
    public static void MapDeleteUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/{userId}", async (string userId, IDeleteUserHandler handler) =>
        {
            await handler.DeleteUserAsync(userId);
            return Results.NoContent();
        })
        .RequireAuthorization("Admin")
        .Produces(StatusCodes.Status204NoContent)
        .WithName("DeleteUser")
        .WithTags("Users");
    }
}
