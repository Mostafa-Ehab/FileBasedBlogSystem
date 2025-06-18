using AutoMapper;
using BlogSystem.Domain.Entities;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.GetPost.DTOs;
using BlogSystem.Features.Users.Data;
using BlogSystem.Infrastructure.MarkdownService;
using BlogSystem.Shared.Exceptions.Posts;

namespace BlogSystem.Features.Posts.Get
{
    public class GetPostHandler : IGetPostHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly MarkdownService _markdownService;
        private readonly IMapper _mapper;

        public GetPostHandler(IPostRepository postRepository, IUserRepository userRepository, MarkdownService markdownService, IMapper mapper)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _markdownService = markdownService;
            _mapper = mapper;
        }

        public Task<GetPostDTO> GetPostAsync(string postSlug)
        {
            var post = _postRepository.GetPostBySlug(postSlug) ?? throw new PostNotFoundException(postSlug);
            post.Content = _markdownService.RenderMarkdown(post.Content);

            var author = _userRepository.GetUserById(post.AuthorId);

            var postDto = _mapper.Map<GetPostDTO>(post);
            postDto.Author = _mapper.Map<PostAuthorDTO>(author);

            return Task.FromResult(postDto);
        }

        public Task<GetPostDTO[]> GetAllPostsAsync(int pageNumber, int pageSize)
        {
            var posts = _postRepository.GetAllPosts(pageNumber, pageSize);
            foreach (var post in posts)
            {
                post.Content = _markdownService.RenderMarkdown(post.Content);
            }
            return Task.FromResult(_mapper.Map<GetPostDTO[]>(posts));
        }
    }
}
