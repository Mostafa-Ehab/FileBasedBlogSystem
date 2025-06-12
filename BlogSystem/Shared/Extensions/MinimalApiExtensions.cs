using BlogSystem.Features.Posts.Get;
using BlogSystem.Features.Categories.GetCategory;
using BlogSystem.Features.Tags.GetTag;
using BlogSystem.Features.Users.Login;
using BlogSystem.Features.Users.CreateUser;

namespace BlogSystem.Shared.Extensions
{
    public static class MinimalApiExtensions
    {
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGroup("/posts").MapPostEndpoints();
            app.MapGroup("/categories").MapCategoryEndpoints();
            app.MapGroup("/tags").MapTagEndpoints();
            app.MapGroup("/users").MapUserEndpoints();

            return app;
        }

        private static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGetPostEndpoint();

            return app;
        }

        private static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGetAllCategoriesEndpoint();
            app.MapGetCategoryEndpoint();
            app.MapGetPostsByCategoryEndpoint();

            return app;
        }

        private static IEndpointRouteBuilder MapTagEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGetAllTagsEndpoint();
            app.MapGetTagEndpoint();
            app.MapGetPostsByTagEndpoint();

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