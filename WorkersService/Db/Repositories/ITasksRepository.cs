using WorkersService.Db.Models;

namespace WorkersService.Db.Repositories;

public interface ITasksRepository
{
    Task<bool> TaskExistsAsync(string taskId);
    Task SaveTaskAsync(TaskModel task);
    Task DeleteTaskAsync(string taskId);
}