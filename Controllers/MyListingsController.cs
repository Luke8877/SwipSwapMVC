using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SwipSwapMVC.Data;
using SwipSwapMVC.Models;
using SwipSwapMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

[Authorize(AuthenticationSchemes = "MyListingsScheme")]
public class MyListingsController : Controller
{
    private readonly AppDbContext _context;

    public MyListingsController(AppDbContext context)
    {
        _context = context;
    }

   
    public IActionResult Index(string search = "", string activeCategory = "all")
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        if (userIdClaim == null) return RedirectToAction("Login", "Account");

        int userId = int.Parse(userIdClaim.Value);

        IQueryable<Product> products = _context.Products
            .Include(p => p.Category)
            .Where(p => p.SellerId == userId);

        if (activeCategory != "all")
            products = products.Where(p => p.Category.Name == activeCategory);

        if (!string.IsNullOrWhiteSpace(search))
            products = products.Where(p => p.Name.Contains(search));

        ViewBag.Categories = _context.Categories.ToList();

        var vm = new MyListingsViewModel
        {
            Products = products.ToList(),
            Search = search,
            NewProduct = new Product() // for the Add form
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult Edit(Product updatedProduct)
    {
        var existing = _context.Products.Find(updatedProduct.ProductId);
        if (existing != null)
        {
            existing.Name = updatedProduct.Name;
            existing.Description = updatedProduct.Description;
            existing.Price = updatedProduct.Price;
            existing.CategoryId = updatedProduct.CategoryId;
            existing.ImageUrl = updatedProduct.ImageUrl;
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Delete(int ProductId)
    {
        var existing = _context.Products.Find(ProductId);
        if (existing != null)
        {
            _context.Products.Remove(existing);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }

  
    [HttpPost]
    public IActionResult Add(Product newProduct)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        if (userIdClaim == null) return RedirectToAction("Login", "Account");

        int userId = int.Parse(userIdClaim.Value);

        if (!ModelState.IsValid)
        {
            // If model validation fails, reload Index with current products
            var vm = new MyListingsViewModel
            {
                Products = _context.Products
                                   .Include(p => p.Category)
                                   .Where(p => p.SellerId == userId)
                                   .ToList(),
                NewProduct = newProduct
            };
            ViewBag.Categories = _context.Categories.ToList();
            return View("Index", vm);
        }

        
        newProduct.SellerId = userId;

        _context.Products.Add(newProduct);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}