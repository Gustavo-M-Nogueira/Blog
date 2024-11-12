using System.Runtime.InteropServices.JavaScript;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _db;
    public CategoryController(ApplicationDbContext db)
    {
        _db = db;
    }
    
    // GET
    public IActionResult Index()
    {
        List<Category> objCategoryList = _db.Categories.ToList();
        return View(objCategoryList);
    }

    public IActionResult Create()
    {
        return View();
    }
    
    // POST
    [HttpPost]
    public IActionResult Create(Category obj)
    {
        if (obj.Name.ToLower() == "test")
        {
            ModelState.AddModelError("name", "Name cannot be test");
        }
        
        if (int.TryParse(obj.Name, out _))
        {
            ModelState.AddModelError("name", "Name cannot be a number");
        }
        
        if(ModelState.IsValid)
        {
            _db.Categories.Add(obj);
            _db.SaveChanges();
            TempData["success"] = "Category created successfully";
            return RedirectToAction("Index");
        }

        return View();
    }
    
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        
        Category? category = _db.Categories.Find(id);
        if (category == null)
        {
            return NotFound();
        }
        
        return View(category);
    }
    
    // POST
    [HttpPost]
    public IActionResult Edit(Category obj)
    {
        if (obj.Name.ToLower() == "test")
        {
            ModelState.AddModelError("name", "Name cannot be test");
        }
        
        if (int.TryParse(obj.Name, out _))
        {
            ModelState.AddModelError("name", "Name cannot be a number");
        }
        
        if(ModelState.IsValid)
        {
            _db.Categories.Update(obj);
            _db.SaveChanges();
            TempData["success"] = "Category updated successfully";
            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        Category? category = _db.Categories.Find(id);
        if (category == null)
        {
            return NotFound();
        }        
        
        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        Category? category = _db.Categories.Find(id);
        if (category == null)
        {
            return NotFound();
        }
        _db.Categories.Remove(category);
        _db.SaveChanges();
        TempData["success"] = "Category deleted successfully";
        return RedirectToAction("Index");
    }
}