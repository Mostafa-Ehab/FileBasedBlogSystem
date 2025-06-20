
using BlogSystem.Features.Posts.SchedulePost.DTOs;

namespace BlogSystem.Features.Posts.SchedulePost
{
    public static class SchedulePostEndpoint
    {
        public static void MapSchedulePostEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/{postId}/schedule", async (ISchedulePostHandler handler, string postId, SchedulePostRequestDTO request) =>
            {
                var result = await handler.SchedulePostAsync(postId, request.ScheduleAt);
                return Results.Ok(result);
            })
            .WithName("SchedulePost")
            .WithTags("Posts")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}