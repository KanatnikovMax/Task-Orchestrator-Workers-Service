namespace WorkersService.Db.Models;

public class TaskModel
{
    public required string TaskId { get; set; }
    public DateTime CreatedAt { get; set; }
}