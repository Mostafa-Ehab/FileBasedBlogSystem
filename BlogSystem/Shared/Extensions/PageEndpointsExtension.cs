namespace BlogSystem.Shared.Extensions;

static class PageEndpointsExtension
{
    public static IEndpointRouteBuilder MapStaticPagesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/posts/{slug}", async context =>
        {
            var filePath = Path.Combine("wwwroot", "blog", "post.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        app.MapGet("/categories/{slug}", async context =>
        {
            var filePath = Path.Combine("wwwroot", "blog", "category.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        app.MapGet("/tags/{slug}", async context =>
        {
            var filePath = Path.Combine("wwwroot", "blog", "tag.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        return app;
    }
}
