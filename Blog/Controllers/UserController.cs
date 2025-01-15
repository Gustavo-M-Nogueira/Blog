using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Controllers;

// [Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: List all users
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();
        return View(users);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        var roles = new List<string> { "User", "Admin" };
        ViewBag.Roles = new SelectList(roles);
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(string email, string username, string password, string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            role = "User"; // Default role
        }

        if (!new[] { "User", "Admin" }.Contains(role))
        {
            ModelState.AddModelError("Role", "Invalid role selected.");
            return View();
        }

        var user = new IdentityUser
        {
            UserName = username,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, role);
            TempData["success"] = "User created successfully!";
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        var roles = new List<string> { "User", "Admin" };
        ViewBag.Roles = new SelectList(roles);
        return View();
    }



    // GET: Edit user
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(IdentityUser user, string? newPassword)
    {
        if (string.IsNullOrEmpty(user.Id))
        {
            return NotFound();
        }

        var userInDb = await _userManager.FindByIdAsync(user.Id);
        if (userInDb == null)
        {
            return NotFound();
        }

        userInDb.UserName = user.UserName;

        if (!string.IsNullOrEmpty(newPassword))
        {
            var passwordValidator = HttpContext.RequestServices.GetService<IPasswordValidator<IdentityUser>>();
            var passwordHasher = HttpContext.RequestServices.GetService<IPasswordHasher<IdentityUser>>();

            var passwordValidationResult = await passwordValidator!.ValidateAsync(_userManager, userInDb, newPassword);
            if (!passwordValidationResult.Succeeded)
            {
                foreach (var error in passwordValidationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(userInDb);
            }

            userInDb.PasswordHash = passwordHasher!.HashPassword(userInDb, newPassword);
        }

        var result = await _userManager.UpdateAsync(userInDb);

        if (result.Succeeded)
        {
            TempData["success"] = "User updated successfully";
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(userInDb);
    }



    // POST: Delete user
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            TempData["success"] = "User deleted successfully";
            return RedirectToAction("Index");
        }

        TempData["error"] = "Error deleting user";
        return RedirectToAction("Index");
    }
}
