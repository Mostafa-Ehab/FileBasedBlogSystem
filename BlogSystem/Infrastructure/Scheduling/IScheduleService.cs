namespace BlogSystem.Infrastructure.Scheduling;

public interface IScheduleService<T>
{
    Task RunTaskAsync(T taskData);
    void RunTask(T taskData);
}