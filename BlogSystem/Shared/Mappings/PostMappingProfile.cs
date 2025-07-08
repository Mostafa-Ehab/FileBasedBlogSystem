using AutoMapper;
using BlogSystem.Domain.Entities;
using BlogSystem.Features.Posts.GetPost.DTOs;

namespace BlogSystem.Shared.Mappings;

public class PostMappingProfile : Profile
{
    public PostMappingProfile()
    {
        CreateMap<Post, GetPostDTO>();
        CreateMap<User, PostAuthorDTO>();
    }
}
