namespace BlogSystem.Shared.Extensions;

static class PageEndpointExtension
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

        app.MapGet("/admin/posts/{slug}/edit", async context =>
        {
            var filePath = Path.Combine("wwwroot", "admin", "edit", "edit-post.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        app.MapGet("/admin/posts/create", async context =>
        {
            var filePath = Path.Combine("wwwroot", "admin", "edit", "edit-post.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        app.MapGet("/admin/users/{userId}/edit", async context =>
        {
            var filePath = Path.Combine("wwwroot", "admin", "edit", "edit-user.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        app.MapGet("/admin/users/create", async context =>
        {
            var filePath = Path.Combine("wwwroot", "admin", "edit", "edit-user.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        return app;
    }
}
