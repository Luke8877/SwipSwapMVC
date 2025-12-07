using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipSwapMVC.Data;
using SwipSwapMVC.Models;
using System.Linq;

namespace SwipSwapMVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string activeCategory = "all", string search = "")
        {
            // Get the logged-in user's ID
            var userIdClaim = User?.FindFirst("id")?.Value;
            int buyerId = !string.IsNullOrEmpty(userIdClaim) ? int.Parse(userIdClaim) : 0;

            // MAIN FEED
            IQueryable<Product> products = _context.Products
                .Include(p => p.Category)
                .Where(p => !p.IsSold); // Hide sold products

            // Category filter
            if (activeCategory != "all")
                products = products.Where(p => p.Category.Name == activeCategory);

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
                products = products.Where(p => p.Name.Contains(search));

            // PAST PURCHASES
            var pastOrders = _context.Orders
                .Include(o => o.Product)
                .Where(o => o.BuyerId == buyerId && o.Status == OrderStatus.Paid)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            ViewBag.PastOrders = pastOrders;
            ViewBag.ActiveCategory = activeCategory;
            ViewBag.Search = search;

            return View(products.ToList());
        }

        public IActionResult ProductDetails()
        {
            return View();
        }
    }
}
