using BlogSystem.Features.Posts.SchedulePost.DTOs;

namespace BlogSystem.Features.Posts.SchedulePost
{
    public interface ISchedulePostHandler
    {
        Task<SchedulePostResponseDTO> SchedulePostAsync(string postId, DateTime scheduleAt);
    }
}