using System.Threading.Tasks;

namespace TasksApi;
public class DataSeeder
{
    public static void Seed(ApiContext context)
    {
        if (context.Users.Any()) return;

        var userId = Guid.Parse("8a44785c-4c85-4e34-9ea3-e904cf2ab4a8");
        var user = new User { Id = userId, Name = "Felipe" };
        context.Users.Add(user);

        var tasks = new List<Task>()
        {
            new Task {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Task1 for Felipe",
                Status = TaskStatus.InProgress },
            new Task {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Task2 for Felipe",
                Status = TaskStatus.NotStarted },
            new Task {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Task3 for Felipe",
                Status = TaskStatus.NotStarted },
        };
        context.Tasks.AddRange(tasks);
        context.SaveChanges();
    }
}