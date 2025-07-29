using BlogSystem.Features.Categories.GetCategory;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.GetPost;
using BlogSystem.Features.Tags.GetTag;
using BlogSystem.Features.Users.GetUser;
using DotNetEnv;

namespace BlogSystem.Shared.Extensions;

static class PageEndpointExtension
{
    public static IEndpointRouteBuilder MapStaticPagesEndpoints(this IEndpointRouteBuilder app)
    {
        #region Home Page
        app.MapGet("/", async context =>
        {
            var filePath = Path.Combine("wwwroot", "index.html");
            var htmlTemplate = await File.ReadAllTextAsync(filePath);

            var finalHtml = htmlTemplate.Replace("{{WEBSITE_URL}}", Environment.GetEnvironmentVariable("WEBSITE_URL") ?? "http://localhost:5000");
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(finalHtml);
        });
        #endregion

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
        app.MapGet("/posts/{slug}", async (HttpContext context, string slug, IGetPostHandler postHandler) =>
        {
            try
            {
                // Fetch the post by slug
                var post = await postHandler.GetPostAsync(slug);

                // Load the HTML template
                var filePath = Path.Combine("wwwroot", "blog", "post.html");
                var htmlTemplate = await File.ReadAllTextAsync(filePath);

                // Generate meta tags based on the slug
                var ogTags = $@"
                    <meta property=""og:title"" content=""{post.Title}"" />
                    <meta property=""og:url"" content=""{Environment.GetEnvironmentVariable("WEBSITE_URL")}/posts/{slug}"" />
                    <meta property=""og:type"" content=""article"" />
                    <meta property=""og:description"" content=""{post.Description}"" />
                    <meta property=""og:image"" content=""{post.ImageUrl}"" />
                ";

                // Replace a placeholder in HTML (e.g., <!--OG_META-->)
                var finalHtml = htmlTemplate.Replace("<!--OG_META-->", ogTags);

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(finalHtml);
            }
            catch (Exception ex)
            {
                // Handle the exception
                var filePath = Path.Combine("wwwroot", "error", "404.html");
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(filePath);
            }
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
        app.MapGet("/categories/{slug}", async (HttpContext context, string slug, IGetCategoryHandler categoryHandler) =>
        {
            try
            {
                // Fetch the category by slug
                var category = await categoryHandler.GetCategoryAsync(slug);


                // Load the HTML template
                var filePath = Path.Combine("wwwroot", "blog", "category.html");
                var htmlTemplate = await File.ReadAllTextAsync(filePath);

                // Generate meta tags based on the slug
                var ogTags = $@"
                    <meta property=""og:title"" content=""{category.Name}"" />
                    <meta property=""og:url"" content=""{Environment.GetEnvironmentVariable("WEBSITE_URL")}/categories/{slug}"" />
                    <meta property=""og:type"" content=""website"" />
                    <meta property=""og:description"" content=""{category.Description}"" />
                    <meta property=""og:image"" content=""/assets/images/blog-thumbnail.png"" />
                ";

                // Replace a placeholder in HTML (e.g., <!--OG_META-->)
                var finalHtml = htmlTemplate.Replace("<!--OG_META-->", ogTags);

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(finalHtml);
            }
            catch (Exception ex)
            {
                // Handle the exception
                var filePath = Path.Combine("wwwroot", "error", "404.html");
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(filePath);
            }
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
        app.MapGet("/tags/{slug}", async (HttpContext context, string slug, IGetTagHandler tagHandler) =>
        {
            try
            {
                // Fetch the tag by slug
                var tag = await tagHandler.GetTagAsync(slug);

                // Load the HTML template
                var filePath = Path.Combine("wwwroot", "blog", "tag.html");
                var htmlTemplate = await File.ReadAllTextAsync(filePath);

                // Generate meta tags based on the slug
                var ogTags = $@"
                    <meta property=""og:title"" content=""{tag.Name}"" />
                    <meta property=""og:url"" content=""{Environment.GetEnvironmentVariable("WEBSITE_URL")}/tags/{slug}"" />
                    <meta property=""og:type"" content=""website"" />
                    <meta property=""og:description"" content=""{tag.Description}"" />
                    <meta property=""og:image"" content=""/assets/images/blog-thumbnail.png"" />
                ";

                // Replace a placeholder in HTML (e.g., <!--OG_META-->)
                var finalHtml = htmlTemplate.Replace("<!--OG_META-->", ogTags);

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(finalHtml);
            }
            catch (Exception ex)
            {
                // Handle the exception
                var filePath = Path.Combine("wwwroot", "error", "404.html");
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(filePath);
            }
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

        app.MapGet("/users/{username}", async (HttpContext context, string username, IGetUserHandler userProfileHandler) =>
        {
            try
            {
                // Fetch the user profile by username
                var userProfile = await userProfileHandler.GetUserAsync(username);

                // Load the HTML template
                var filePath = Path.Combine("wwwroot", "blog", "profile.html");
                var htmlTemplate = await File.ReadAllTextAsync(filePath);

                // Generate meta tags based on the username
                var ogTags = $@"
                    <meta property=""og:title"" content=""{userProfile.FullName}"" />
                    <meta property=""og:url"" content=""{Environment.GetEnvironmentVariable("WEBSITE_URL")}/users/{username}"" />
                    <meta property=""og:type"" content=""profile"" />
                    <meta property=""og:description"" content=""Profile of {userProfile.FullName}"" />
                    <meta property=""og:image"" content=""{userProfile.ProfilePictureUrl}"" />
                ";

                // Replace a placeholder in HTML (e.g., <!--OG_META-->)
                var finalHtml = htmlTemplate.Replace("<!--OG_META-->", ogTags);

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(finalHtml);
            }
            catch (Exception ex)
            {
                // Handle the exception
                var filePath = Path.Combine("wwwroot", "error", "404.html");
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(filePath);
            }
        });
        #endregion

        return app;
    }
}
