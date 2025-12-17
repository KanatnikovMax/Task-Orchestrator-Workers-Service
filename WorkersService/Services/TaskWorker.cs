using WorkersService.Db.Repositories;

namespace WorkersService.Services;

public class TaskWorker(
    WorkerProgressNotifier progressNotifier, 
    ITasksRepository repository,
    ILogger<TaskWorker> logger)
{
    public async Task ProcessTaskAsync(string taskId, CancellationToken cancellationToken)
    {
        try
        {
            if (!await repository.TaskExistsAsync(taskId, cancellationToken))
            {
                logger.LogInformation("Task {TaskId} doesn't exist", taskId);
                return;
            }
            
            await progressNotifier.StartAsync(cancellationToken);
            
            await progressNotifier.NotifyProgressAsync(taskId, 0, cancellationToken);
            
            await Task.Delay(7000, cancellationToken);
            await progressNotifier.NotifyProgressAsync(taskId, 25, cancellationToken);
            
            await Task.Delay(7000, cancellationToken);
            await progressNotifier.NotifyProgressAsync(taskId, 50, cancellationToken);
            
            await Task.Delay(7000, cancellationToken);
            await progressNotifier.NotifyProgressAsync(taskId, 75, cancellationToken);
            
            await Task.Delay(7000, cancellationToken);
            await progressNotifier.NotifyProgressAsync(taskId, 100, cancellationToken);
            
            await repository.DeleteTaskAsync(taskId, cancellationToken);
            
            logger.LogInformation("Task {TaskId} completed successfully", taskId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing task {TaskId}", taskId);
            await progressNotifier.NotifyProgressAsync(taskId, 0, cancellationToken);
        }
    }
}