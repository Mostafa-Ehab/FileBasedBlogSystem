using BlogSystem.Features.Posts.Get;
using BlogSystem.Features.Categories.GetCategory;
using BlogSystem.Features.Tags.GetTag;
using BlogSystem.Features.Users.Login;
using BlogSystem.Features.Users.CreateUser;
using BlogSystem.Features.Categories.CreateCategory;
using BlogSystem.Features.Tags.CreateTag;
using BlogSystem.Features.Posts.RSS;
using BlogSystem.Features.Users.GetUser;
using BlogSystem.Features.Posts.PostManagement;

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
            app.MapGroup("/api/auth").MapAuthEndpoints();
            app.MapRSSEndpoint();

            return app;
        }

        private static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGetPostEndpoint();
            app.MapPostManagementEndpoints();
            app.MapGetAllPostsEndpoint();

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
            app.MapGetUserEndpoint();
            app.MapCreateUserEndpoint();

            return app;
        }

        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapLoginEndpoint();

            return app;
        }
    }
}