using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Sport" },
            new Category { Id = 2, Name = "Cinema" },
            new Category { Id = 3, Name = "Games" }
        );
    }
}