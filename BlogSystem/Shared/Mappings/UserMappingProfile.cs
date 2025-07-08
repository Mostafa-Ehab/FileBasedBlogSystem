using AutoMapper;
using BlogSystem.Domain.Entities;
using BlogSystem.Features.Users.GetUser.DTOs;

namespace BlogSystem.Shared.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, GetUserDTO>();
    }
}
