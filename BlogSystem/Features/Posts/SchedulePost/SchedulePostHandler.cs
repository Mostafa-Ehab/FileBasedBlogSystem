using BlogSystem.Domain.Entities;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.SchedulePost.DTOs;
using BlogSystem.Infrastructure.Scheduling;
using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Exceptions.Posts;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace BlogSystem.Features.Posts.SchedulePost
{
    public class SchedulePostHandler : ISchedulePostHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly IScheduler _scheduler;
        public SchedulePostHandler(IPostRepository postRepository, IScheduler scheduler)
        {
            _postRepository = postRepository;
            _scheduler = scheduler;
        }

        public async Task<SchedulePostResponseDTO> SchedulePostAsync(string postId, DateTime scheduleAt)
        {
            if (scheduleAt <= DateTime.UtcNow)
            {
                throw new ValidationErrorException("Schedule time must be in the future.");
            }

            var post = _postRepository.GetPostById(postId) ?? throw new PostNotFoundException(postId);
            if (post.IsPublished)
            {
                throw new ValidationErrorException("Cannot schedule a post that is already published.");
            }

            ValidatePost(post);

            if (post.ScheduledAt.HasValue && post.ScheduleToken != null)
            {
                Console.WriteLine($"Cancelling existing scheduled job for post {postId}");
                _scheduler.CancelJob(post.ScheduleToken);
            }

            post.ScheduleToken = _scheduler.ScheduleJob<PublishPostService>(
                _publishPostService => _publishPostService.RunTask(postId),
                scheduleAt
            );
            var content = post.Content!;
            post.Content = null; // Clear content to avoid saving in metadata
            post.ScheduledAt = scheduleAt;

            _postRepository.UpdatePost(post, content);

            Console.WriteLine($"Scheduled job ID: {post.ScheduleToken}");
            return await Task.FromResult(
                new SchedulePostResponseDTO
                {
                    ScheduledAt = post.ScheduledAt.Value
                }
            );
        }

        private void ValidatePost(Post post)
        {
            if (post == null)
            {
                throw new ValidationErrorException("Post cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(post.Title) ||
                string.IsNullOrWhiteSpace(post.Description) ||
                string.IsNullOrWhiteSpace(post.Category) ||
                string.IsNullOrWhiteSpace(post.ImageUrl) ||
                string.IsNullOrWhiteSpace(post.Content))
            {
                throw new ValidationErrorException("Post must have a title, description, category, image URL, and content.");
            }
        }
    }
}