using BlogSystem.Features.Posts.Data;
using BlogSystem.Infrastructure.Scheduling;

namespace BlogSystem.Features.Posts.SchedulePost
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

            var content = post.Content;
            post.IsPublished = true;
            post.Content = null;
            _postRepository.UpdatePost(post, content!);
        }

        public async Task RunTaskAsync(string postId)
        {
            await Task.Run(() => RunTask(postId));
        }
    }
}