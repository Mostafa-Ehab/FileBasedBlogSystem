using BlogSystem.Features.Posts.Get;
using BlogSystem.Features.Categories.GetCategory;
using BlogSystem.Features.Tags.GetTag;
using BlogSystem.Features.Users.Login;
using BlogSystem.Features.Users.CreateUser;
using BlogSystem.Features.Posts.Create;
using BlogSystem.Features.Posts.UpdatePost;
using BlogSystem.Features.Categories.CreateCategory;
using BlogSystem.Features.Tags.CreateTag;
using BlogSystem.Features.Posts.RSS;
using BlogSystem.Features.Posts.SchedulePost;

namespace BlogSystem.Shared.Extensions
{
    public static class MinimalApiExtensions
    {
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGroup("/api/posts").MapPostEndpoints();
            app.MapGroup("/api/categories").MapCategoryEndpoints();
            app.MapGroup("/api/tags").MapTagEndpoints();
            app.MapGroup("/api/users").MapUserEndpoints();
            app.MapRSSEndpoint();

            return app;
        }

        private static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGetPostEndpoint();
            app.MapGetAllPostsEndpoint();
            app.MapCreatePostEndpoint();
            app.MapUpdatePostEndpoint();
            app.MapSearchPostsEndpoint();
            app.MapSchedulePostEndpoints();

            return app;
        }

        private static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGetAllCategoriesEndpoint();
            app.MapGetCategoryEndpoint();
            app.MapGetPostsByCategoryEndpoint();
            app.MapCreateCategoryEndpoint();

            return app;
        }

        private static IEndpointRouteBuilder MapTagEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGetAllTagsEndpoint();
            app.MapGetTagEndpoint();
            app.MapGetPostsByTagEndpoint();
            app.MapCreateTagEndpoint();

            return app;
        }

        public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapLoginEndpoint();
            app.MapCreateUserEndpoint();

            return app;
        }
    }
}