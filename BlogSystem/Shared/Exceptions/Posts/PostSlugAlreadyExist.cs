using System;

namespace BlogSystem.Shared.Exceptions.Posts;

public class PostSlugAlreadyExistException : ApplicationCustomException
{
    public PostSlugAlreadyExistException(string slug)
        : base($"A post with the slug '{slug}' already exists.", 409, 409)
    {
    }
}