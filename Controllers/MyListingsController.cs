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
    public IActionResult Edit(Product updatedProduct, IFormFile? NewImageFile)
    {
        var existing = _context.Products.Find(updatedProduct.ProductId);
        if (existing == null)
            return RedirectToAction("Index");

        existing.Name = updatedProduct.Name;
        existing.Description = updatedProduct.Description;
        existing.Price = updatedProduct.Price;
        existing.CategoryId = updatedProduct.CategoryId;

        // if a new image was uploaded
        if (NewImageFile != null && NewImageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(NewImageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                NewImageFile.CopyTo(stream);
            }

            existing.ImageUrl = "/images/" + fileName;
        }

        _context.SaveChanges();
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
    public IActionResult Add(Product newProduct, IFormFile? ImageFile)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        if (userIdClaim == null) return RedirectToAction("Login", "Account");

        int userId = int.Parse(userIdClaim.Value);

        // built-in validation for required fields
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = _context.Categories.ToList();
            var vm = new MyListingsViewModel
            {
                Products = _context.Products
                                   .Include(p => p.Category)
                                   .Where(p => p.SellerId == userId)
                                   .ToList(),
                NewProduct = newProduct
            };
            return View("Index", vm);
        }

        // custom validation for uploaded image
        if (ImageFile != null && ImageFile.Length > 0)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
            var uploadPath = Path.Combine("wwwroot/uploads", fileName);

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                ImageFile.CopyTo(stream);
            }

            newProduct.ImageUrl = "/uploads/" + fileName;
        }


        newProduct.SellerId = userId;
        _context.Products.Add(newProduct);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}