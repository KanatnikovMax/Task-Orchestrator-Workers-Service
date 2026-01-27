using WorkersService.Db.Models;

namespace WorkersService.Db.Repositories;

public interface ITasksRepository
{
    Task<bool> TaskExistsAsync(string taskId, CancellationToken cancellationToken);
    Task SaveTaskAsync(TaskModel task, CancellationToken cancellationToken);
    Task DeleteTaskAsync(string taskId, CancellationToken cancellationToken);
}