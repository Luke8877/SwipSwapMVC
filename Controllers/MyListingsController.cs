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
            NewProduct = new Product()
        };

        return View(vm);
    }


    [HttpPost]
    public async Task<IActionResult> Add(Product newProduct)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        if (userIdClaim == null) return RedirectToAction("Login", "Account");

        int userId = int.Parse(userIdClaim.Value);

        if (!ModelState.IsValid)
        {
            var vm = new MyListingsViewModel
            {
                Products = _context.Products.Include(p => p.Category)
                                            .Where(p => p.SellerId == userId)
                                            .ToList(),
                NewProduct = newProduct
            };
            ViewBag.Categories = _context.Categories.ToList();
            return View("Index", vm);
        }

        newProduct.SellerId = userId;

        // Auto-geocode if address provided & no coords manually set
        if (!string.IsNullOrWhiteSpace(newProduct.PickupAddress)
            && (!newProduct.Latitude.HasValue || !newProduct.Longitude.HasValue))
        {
            try
            {
                using var client = new HttpClient();
                var url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(newProduct.PickupAddress)}";

                client.DefaultRequestHeaders.Add("User-Agent", "SwipSwap App");

                var response = await client.GetStringAsync(url);
                var geo = System.Text.Json.JsonSerializer.Deserialize<List<GeoResponse>>(response);

                if (geo != null && geo.Count > 0)
                {
                    newProduct.Latitude = double.Parse(geo[0].lat);
                    newProduct.Longitude = double.Parse(geo[0].lon);
                }
            }
            catch
            {
                // Silent fallback - address remains text only
            }
        }

        _context.Products.Add(newProduct);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public class GeoResponse
    {
        public string lat { get; set; }
        public string lon { get; set; }
    }


    [HttpPost]
    public IActionResult Edit(Product updatedProduct)
    {
        var existing = _context.Products.Find(updatedProduct.ProductId);
        if (existing == null) return RedirectToAction("Index");

        // Verify user owns this listing
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        int userId = int.Parse(userIdClaim.Value);

        if (existing.SellerId != userId) return Unauthorized();

        existing.Name = updatedProduct.Name;
        existing.Description = updatedProduct.Description;
        existing.Price = updatedProduct.Price;
        existing.CategoryId = updatedProduct.CategoryId;
        existing.ImageUrl = updatedProduct.ImageUrl;

        existing.PickupAddress = updatedProduct.PickupAddress;
        existing.SellerPhone = updatedProduct.SellerPhone;
        existing.Latitude = updatedProduct.Latitude;
        existing.Longitude = updatedProduct.Longitude;

        _context.SaveChanges();
        return RedirectToAction("Index");
    }


    [HttpPost]
    public IActionResult Delete(int ProductId)
    {
        var existing = _context.Products.Find(ProductId);
        if (existing == null) return RedirectToAction("Index");

        // Verify owner before delete
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        int userId = int.Parse(userIdClaim.Value);

        if (existing.SellerId != userId) return Unauthorized();

        _context.Products.Remove(existing);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}
