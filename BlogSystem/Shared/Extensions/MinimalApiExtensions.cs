using BlogSystem.Features.Categories.CreateCategory;
using BlogSystem.Features.Categories.GetCategory;
using BlogSystem.Features.Posts.GetPost;
using BlogSystem.Features.Posts.PostManagement;
using BlogSystem.Features.Posts.RSS;
using BlogSystem.Features.Tags.CreateTag;
using BlogSystem.Features.Tags.GetTag;
using BlogSystem.Features.Users.CreateUser;
using BlogSystem.Features.Users.DeleteUser;
using BlogSystem.Features.Users.GetUser;
using BlogSystem.Features.Users.Login;
using BlogSystem.Features.Users.UpdateUser;

namespace BlogSystem.Shared.Extensions;

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
        app.MapStaticPagesEndpoints();

        return app;
    }

    private static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetPostEndpoint();
        app.MapPostManagementEndpoints();
        app.MapGetAllPostsEndpoint();
        app.MapUpdatePostEndpoint();
        app.MapDeletePostEndpoint();

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
        app.MapGetUserEndpoint();
        app.MapGetMyProfileEndpoint();
        app.MapCreateUserEndpoint();
        app.MapUpdateUserEndpoints();
        app.MapDeleteUserEndpoint();

        return app;
    }

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapLoginEndpoint();

        return app;
    }
}