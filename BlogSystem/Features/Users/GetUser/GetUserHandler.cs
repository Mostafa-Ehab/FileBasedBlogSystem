using AutoMapper;
using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.GetUser.DTOs;

namespace BlogSystem.Features.Users.GetUser;

public class GetUserHandler : IGetUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public Task<GetUserDTO[]> GetAllUsers()
    {
        var users = _userRepository.GetAllUsers();
        return Task.FromResult(_mapper.Map<GetUserDTO[]>(users));
    }
}
