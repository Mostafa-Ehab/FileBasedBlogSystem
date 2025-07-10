using BlogSystem.Domain.Enums;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Infrastructure.Scheduling;

namespace BlogSystem.Features.Posts.PostManagement
{
    public class PublishPostService : IScheduleService<string>
    {
        private readonly IPostRepository _postRepository;

        public PublishPostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public void RunTask(string postId)
        {
            var post = _postRepository.GetPostById(postId);

            if (post == null)
            {
                Console.WriteLine($"Post with ID {postId} not found.");
                return;
            }

            post.Status = PostStatus.Published;
            post.PublishedAt = DateTime.UtcNow;
            _postRepository.UpdatePost(post);
        }

        public async Task RunTaskAsync(string postId)
        {
            await Task.Run(() => RunTask(postId));
        }
    }
}