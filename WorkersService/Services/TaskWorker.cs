using WorkersService.Db.Repositories;

namespace WorkersService.Services;

public class TaskWorker(
    WorkerProgressNotifier progressNotifier, 
    ITasksRepository repository,
    ILogger<TaskWorker> logger)
{
    public async Task ProcessTaskAsync(string taskId)
    {
        try
        {
            if (!await repository.TaskExistsAsync(taskId))
            {
                logger.LogInformation("Task {TaskId} doesn't exist", taskId);
                return;
            }
            
            await progressNotifier.StartAsync();
            
            await progressNotifier.NotifyProgressAsync(taskId, 0);
            
            await Task.Delay(7000);
            await progressNotifier.NotifyProgressAsync(taskId, 25);
            
            await Task.Delay(7000);
            await progressNotifier.NotifyProgressAsync(taskId, 50);
            
            await Task.Delay(7000);
            await progressNotifier.NotifyProgressAsync(taskId, 75);
            
            await Task.Delay(7000);
            await progressNotifier.NotifyProgressAsync(taskId, 100);
            
            await repository.DeleteTaskAsync(taskId);
            
            logger.LogInformation("Task {TaskId} completed successfully", taskId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing task {TaskId}", taskId);
            await progressNotifier.NotifyProgressAsync(taskId, 0);
        }
    }
}