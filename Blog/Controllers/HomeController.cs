using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string? searchQuery, string[]? year, int[]? categoryId)
        {
            var posts = _db.Posts.Include(p => p.Category).AsQueryable();

            // Apply year filter only if specific years are selected and "All" is not selected
            if (year != null && year.Any(y => !string.IsNullOrEmpty(y)))
            {
                var selectedYears = year
                    .Where(y => !string.IsNullOrEmpty(y)) // Filter out empty strings
                    .Select(y => int.Parse(y)) // Convert remaining non-empty strings to integers
                    .ToList();

                posts = posts.Where(p => selectedYears.Contains(p.CreatedAt.Year) || 
                                         (p.UpdatedAt.HasValue && selectedYears.Contains(p.UpdatedAt.Value.Year)));
            }

            // Apply category filter (only if specific categories are selected and "All" is not selected)
            if (categoryId != null && categoryId.Any() && !categoryId.Contains(0))
            {
                posts = posts.Where(p => categoryId.Contains(p.CategoryId));
            }

            // Apply search query filter if it's provided
            if (!string.IsNullOrEmpty(searchQuery))
            {
                posts = posts.Where(p => p.Title.Contains(searchQuery) || p.Content.Contains(searchQuery));
            }

            // Load categories for the filter dropdown
            ViewBag.Categories = _db.Categories.ToList();

            // Pass available years for filtering
            var availableYears = _db.Posts
                .Where(p => p.CreatedAt != null || p.UpdatedAt != null)
                .Select(p => p.CreatedAt.Year)
                .Concat(_db.Posts.Where(p => p.UpdatedAt != null).Select(p => p.UpdatedAt.Value.Year))
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            ViewBag.Years = availableYears;

            // Pass selected filters back to the view to keep them selected
            ViewBag.SelectedYears = year?.ToList();
            ViewBag.SelectedCategories = categoryId?.Select(c => c.ToString()).ToList();
            ViewBag.SearchQuery = searchQuery;

            return View(posts.ToList());
        }
    }
}
