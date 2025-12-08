using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipSwapMVC.Data;
using SwipSwapMVC.Models;

namespace SwipSwapMVC.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly AppDbContext _db;

        public PaymentsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> History()
        {
            // Try to read user id from JWT / cookie
            var idClaim = User?.FindFirst("id")?.Value;

            int userId;
            if (!string.IsNullOrEmpty(idClaim))
            {
                userId = int.Parse(idClaim);
            }
            else
            {
                // Demo fallback: treat as user 1 (same idea as your Checkout controller)
                userId = 1;
            }

            var orders = await _db.Orders
                .Include(o => o.Product)
                .Include(o => o.Payment)
                .Where(o => o.BuyerId == userId && o.Status == OrderStatus.Paid)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View("~/Views/Payments/History.cshtml", orders);
        }
    }
}
