using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

namespace Blog.Models;

public class User : IdentityUser
{
    [Required]
    public string Name { get; set; }

    [NotMapped]
    public string Role { get; set; }
}