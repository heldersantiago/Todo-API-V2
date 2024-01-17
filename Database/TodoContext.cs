using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

public class TodoContext : IdentityDbContext<IdentityUser>
{
    public virtual DbSet<TodoItem> TodoItems { get; set; }
    public virtual DbSet<User> Users { get; set; }

    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            // Add other properties and configurations as needed
        });
    }
}
