using Microsoft.EntityFrameworkCore;
using WorkersService.Db.Models;

namespace WorkersService.Db.Repositories;

public class TasksRepository(IDbContextFactory<WorkersDbContext> dbContextFactory) : ITasksRepository
{
    public async Task<bool> TaskExistsAsync(string taskId, CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Tasks.AnyAsync(x => x.TaskId == taskId, cancellationToken);
    }

    public async Task SaveTaskAsync(TaskModel task, CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        if (!await context.Tasks.AnyAsync(x => x.TaskId == task.TaskId, cancellationToken))
        {
            await context.Tasks.AddAsync(task, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteTaskAsync(string taskId, CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var task =  await context.Tasks.FirstOrDefaultAsync(x => x.TaskId == taskId, cancellationToken);
        if (task != null)
        {
            context.Tasks.Remove(task);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}