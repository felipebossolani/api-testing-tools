namespace TasksApi;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<TTask> Tasks { get; set; } = new List<TTask>();
}

public class TTask
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string Name { get; set; }
    public TaskStatus Status { get; set; }
}
