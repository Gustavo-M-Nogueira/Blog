using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Blog.Models;

public class Post
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }
    
    [Required]
    public string Content { get; set; }

    [ForeignKey("Category")]
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; }
    public virtual IdentityUser User { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }
}
