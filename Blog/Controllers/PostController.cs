using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

public class PostController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;
    
    public PostController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    private async Task<bool> IsAdminAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        return await _userManager.IsInRoleAsync(user, "Admin");
    }

    public IActionResult Create()
    {
        // Check if the user is logged in
        if (!User.Identity.IsAuthenticated)
        {
            // Redirect to the login page if the user is not authenticated
            return Redirect("http://localhost:5277/Identity/Account/Login");
        }

        // Populate categories for the dropdown
        ViewData["Categories"] = _db.Categories.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Post post)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            post.UserId = userId;
            post.CreatedAt = DateTime.Now;

            _db.Add(post);
            await _db.SaveChangesAsync();

            TempData["success"] = "Post criado com sucesso!";
            return RedirectToAction("Index", "Home"); 
        }
        catch
        {
            TempData["error"] = "Ocorreu um erro ao criar o post.";
            return View(post);
        }
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var post = _db.Posts.Include(p => p.User).FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            return NotFound();
        }

        ViewData["Categories"] = _db.Categories.ToList();

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewData["CurrentUserId"] = currentUserId;

        bool isAdmin = await IsAdminAsync(); // Verifica se o usuário é admin
        ViewData["IsAdmin"] = isAdmin;

        return View(post);
    }

    // POST: Edit Post
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Post post)
    {
        try
        {
            var existingPost = await _db.Posts.FindAsync(post.Id);

            if (existingPost == null)
            {
                TempData["error"] = "Post not found.";
                return RedirectToAction("Index", "Home");
            }

            // Update properties
            existingPost.Title = post.Title;
            existingPost.Content = post.Content;
            existingPost.CategoryId = post.CategoryId;
            existingPost.UpdatedAt = DateTime.Now;

            // Save changes
            _db.Posts.Update(existingPost);
            await _db.SaveChangesAsync();

            TempData["success"] = "Post updated successfully!";
            return RedirectToAction("Index", "Home");
        }
        catch
        {
            TempData["error"] = "An error occurred while updating the post.";
            return View(post);
        }
    }

    // DELETE: Post/Delete/{id}
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post == null)
        {
            TempData["error"] = "Post not found.";
            return RedirectToAction("Index", "Home");
        }

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();

        TempData["success"] = "Post deleted successfully!";
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var post = await _db.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
        if (post == null)
        {
            return NotFound();
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewData["CurrentUserId"] = currentUserId;

        bool isAdmin = await IsAdminAsync(); 
        ViewData["IsAdmin"] = isAdmin;

        return View(post);
    }
}
