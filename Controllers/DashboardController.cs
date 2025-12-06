using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipSwapMVC.Data;
using SwipSwapMVC.Models;

namespace swipswapmvc.Controllers
{
    public class DashboardController : Controller
    {
        private AppDbContext _context;
        public DashboardController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string activeCategory = "all")
        {
            ViewBag.ActiveCategory = activeCategory; // the current product category

            IQueryable<Product> products = _context.Products.Include(p => p.Category); //returns all products joined with category
            
            // if category is not all, filter to products with active category names
            if (activeCategory != "all")
            {
                products = products.Where(
                    p => p.Category.Name == activeCategory);
            }
           
            return View(products.ToList());
        }

        public IActionResult ProductDetails()
        {
            return View();
        }
    }
}
