using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser> 
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Sport" },
            new Category { Id = 2, Name = "Cinema" },
            new Category { Id = 3, Name = "Games" }
        );

        var hasher = new PasswordHasher<IdentityUser>();
        
        var user1 = new IdentityUser
        {
            Id = "user1-id", 
            UserName = "author1",
            NormalizedUserName = "AUTHOR1@EXAMPLE.COM",
            Email = "author1@example.com",
            NormalizedEmail = "AUTHOR1@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "Password123!")
        };
        
        var user2 = new IdentityUser
        {
            Id = "user2-id",
            UserName = "author2",
            NormalizedUserName = "AUTHOR2@EXAMPLE.COM",
            Email = "author2@example.com",
            NormalizedEmail = "AUTHOR2@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "Password123!")
        };
        
        modelBuilder.Entity<IdentityUser>().HasData(user1, user2);

        modelBuilder.Entity<Post>().HasData(
            new Post
            {
                Id = 1,
                Title = "The Rise of Esports",
                Content = "An in-depth look into the world of competitive gaming.",
                CreatedAt = new DateTime(2019, 7, 15),
                UpdatedAt = null,
                CategoryId = 1, // Sport
                UserId = user1.Id
            },
            new Post
            {
                Id = 2,
                Title = "Top 10 Movies of 2020",
                Content = "A review of the best movies released in 2020.",
                CreatedAt = new DateTime(2020, 12, 20),
                UpdatedAt = null,
                CategoryId = 2, // Cinema
                UserId = user2.Id
            },
            new Post
            {
                Id = 3,
                Title = "The Best Games of 2021",
                Content = "A list of the most popular games of 2021.",
                CreatedAt = new DateTime(2021, 11, 5),
                UpdatedAt = null,
                CategoryId = 3, // Games
                UserId = user1.Id
            },
            new Post
            {
                Id = 4,
                Title = "Future Trends in Sports Technology",
                Content = "Exploring the upcoming trends in sports tech for 2022.",
                CreatedAt = new DateTime(2022, 3, 10),
                UpdatedAt = null,
                CategoryId = 1, // Sport
                UserId = user2.Id
            },
            new Post
            {
                Id = 5,
                Title = "2023 Movie Predictions",
                Content = "What movies to look out for in 2023.",
                CreatedAt = new DateTime(2023, 1, 25),
                UpdatedAt = null,
                CategoryId = 2, // Cinema
                UserId = user1.Id
            }
        );
    }
}
