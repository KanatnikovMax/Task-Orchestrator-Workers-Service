using Microsoft.EntityFrameworkCore;
using WorkersService.Db.Models;

namespace WorkersService.Db.Repositories;

public class TasksRepository(IDbContextFactory<WorkersDbContext> dbContextFactory) : ITasksRepository
{
    public async Task<bool> TaskExistsAsync(string taskId)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        return await context.Tasks.AnyAsync(x => x.TaskId == taskId);
    }

    public async Task SaveTaskAsync(TaskModel task)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        
        if (!await context.Tasks.AnyAsync(x => x.TaskId == task.TaskId))
        {
            await context.Tasks.AddAsync(task);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteTaskAsync(string taskId)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        
        var task =  await context.Tasks.FirstOrDefaultAsync(x => x.TaskId == taskId);
        if (task != null)
        {
            context.Tasks.Remove(task);
            await context.SaveChangesAsync();
        }
    }
}