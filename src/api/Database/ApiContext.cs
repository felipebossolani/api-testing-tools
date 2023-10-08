using Microsoft.EntityFrameworkCore;

namespace TasksApi;

public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Task> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId);
    }
}
