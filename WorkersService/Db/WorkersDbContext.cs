using Microsoft.EntityFrameworkCore;
using WorkersService.Db.Models;

namespace WorkersService.Db;

public class WorkersDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<TaskModel> Tasks { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskModel>().HasKey(x => x.TaskId);
    }
}