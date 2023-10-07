namespace TasksApi;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}

public class Task
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string Name { get; set; }
    public TaskStatus Status { get; set; }
}
