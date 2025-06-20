using System.Linq.Expressions;
using Hangfire;

namespace BlogSystem.Infrastructure.Scheduling
{
    public class HangfireScheduler : IScheduler
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IServiceProvider _serviceProvider;
        public HangfireScheduler(IBackgroundJobClient backgroundJobClient, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _backgroundJobClient = backgroundJobClient;
        }
        public string ScheduleJob<T>(Expression<Action<T>> job, DateTime runAt) where T : class
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var jobService = scope.ServiceProvider.GetRequiredService<T>();
                var jobId = _backgroundJobClient.Schedule(job, runAt);
                return jobId;
            }
        }

        public void CancelJob(string jobId)
        {
            _backgroundJobClient.Delete(jobId);
        }
    }
}