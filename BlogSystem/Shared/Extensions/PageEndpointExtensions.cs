namespace BlogSystem.Shared.Extensions;

static class PageEndpointExtension
{
    public static IEndpointRouteBuilder MapStaticPagesEndpoints(this IEndpointRouteBuilder app)
    {
        #region User Pages
        app.MapGet("/admin/users", async context =>
        {
            var filePath = Path.Combine("wwwroot", "admin", "users.html");
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
        #endregion

        #region Post Pages
        app.MapGet("/posts/{slug}", async context =>
        {
            var filePath = Path.Combine("wwwroot", "blog", "post.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        app.MapGet("/admin/posts", async context =>
        {
            var filePath = Path.Combine("wwwroot", "admin", "posts.html");
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
        #endregion

        #region Category Pages
        app.MapGet("/categories/{slug}", async context =>
        {
            var filePath = Path.Combine("wwwroot", "blog", "category.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        app.MapGet("/admin/categories", async context =>
        {
            var filePath = Path.Combine("wwwroot", "admin", "categories.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        app.MapGet("/admin/categories/{slug}/edit", async context =>
        {
            var filePath = Path.Combine("wwwroot", "admin", "edit", "edit-category.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        app.MapGet("/admin/categories/create", async context =>
        {
            var filePath = Path.Combine("wwwroot", "admin", "edit", "edit-category.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });
        #endregion

        #region Tag Pages
        app.MapGet("/tags/{slug}", async context =>
        {
            var filePath = Path.Combine("wwwroot", "blog", "tag.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        app.MapGet("/admin/tags", async context =>
        {
            var filePath = Path.Combine("wwwroot", "admin", "tags.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        app.MapGet("/admin/tags/{slug}/edit", async context =>
        {
            var filePath = Path.Combine("wwwroot", "admin", "edit", "edit-tag.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        app.MapGet("/admin/tags/create", async context =>
        {
            var filePath = Path.Combine("wwwroot", "admin", "edit", "edit-tag.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });
        #endregion

        #region Author Pages
        app.MapGet("/admin/profile", async context =>
        {
            var filePath = Path.Combine("wwwroot", "admin", "profile.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });

        app.MapGet("/users/{username}", async context =>
        {
            var filePath = Path.Combine("wwwroot", "blog", "profile.html");
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(filePath);
        });
        #endregion

        return app;
    }
}
