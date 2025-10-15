namespace WorkersService.Services;

public class TaskWorker(WorkerProgressNotifier progressNotifier, ILogger<TaskWorker> logger)
{
    public async Task ProcessTaskAsync(string taskId)
    {
        try
        {
            await progressNotifier.StartAsync();
            
            await progressNotifier.NotifyProgressAsync(taskId, 0);
            
            await Task.Delay(2000);
            await progressNotifier.NotifyProgressAsync(taskId, 25);
            
            await Task.Delay(2000);
            await progressNotifier.NotifyProgressAsync(taskId, 50);
            
            await Task.Delay(2000);
            await progressNotifier.NotifyProgressAsync(taskId, 75);
            
            await Task.Delay(2000);
            await progressNotifier.NotifyProgressAsync(taskId, 100);
            
            logger.LogInformation("Task {TaskId} completed successfully", taskId);
            
            await progressNotifier.StopAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing task {TaskId}", taskId);
            await progressNotifier.NotifyProgressAsync(taskId, 0);
        }
    }
}