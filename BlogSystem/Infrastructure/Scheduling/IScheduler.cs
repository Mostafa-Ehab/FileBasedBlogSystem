using System.Linq.Expressions;

namespace BlogSystem.Infrastructure.Scheduling
{
    public interface IScheduler
    {
        string ScheduleJob<T>(Expression<Action<T>> job, DateTime runAt) where T : class;
        void CancelJob(string jobId);
    }
}