using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using SwipSwapMVC.Data;
using SwipSwapMVC.Models;
using SwipSwapMVC.Models.ViewModels;
using static SwipSwapMVC.Models.ViewModels.CheckoutSuccessViewModel;

namespace SwipSwapMVC.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly AppDbContext _db;

        public CheckoutController(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Creates an order, initializes a payment record, and generates
        /// a Stripe Checkout session for the selected product.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int productId)
        {
            int buyerId;
    
            var idClaim = User?.FindFirst("id")?.Value;

            if (!string.IsNullOrEmpty(idClaim))
            {
                buyerId = int.Parse(idClaim);
            }
            else
            {
               
                buyerId = 1;
            }


            // Fetch the product being purchased.
            var product = await _db.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
                return NotFound("Product not found.");

            if (product.IsSold)
                return BadRequest("Product already sold.");

            // Create a pending order linked to the buyer and product.
            var order = new Order
            {
                BuyerId = buyerId,
                ProductId = product.ProductId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // Initialize the payment record in a pending state.
            var payment = new Payment
            {
                OrderId = order.OrderId,
                PaymentStatus = "Pending",
                Amount = product.Price,
                CreatedAt = DateTime.UtcNow
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            // Configure the Stripe Checkout session.
            var domain = $"{Request.Scheme}://{Request.Host}";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",

                // Stripe redirects
                SuccessUrl = $"{domain}/Checkout/Success?orderId={order.OrderId}",
                CancelUrl = $"{domain}/Checkout/Cancel?orderId={order.OrderId}",

                // Product details for Stripe UI
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "cad",
                            UnitAmount = (long)(product.Price * 100), // Stripe expects amount in cents
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = product.Name
                            }
                        }
                    }
                },

                // Used by webhook to match Stripe payment → internal order
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", order.OrderId.ToString() }
                }
            };

            var service = new SessionService();
            var session = service.Create(options);

            // Persist Stripe session identifiers for later webhook validation.
            payment.SessionId = session.Id;
            payment.PaymentIntentId = session.PaymentIntentId;
            await _db.SaveChangesAsync();

            // Send the customer to Stripe's hosted payment page.
            return Redirect(session.Url);
        }

        /// <summary>
        /// Displays confirmation after Stripe completes payment.
        /// Actual order finalization happens with webhook.
        /// </summary>
        public async Task<IActionResult> Success(int orderId)
        {
            var order = await _db.Orders
                .Include(o => o.Product)
                .ThenInclude(p => p.Seller)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return NotFound("Order not found");

            // Mark as Paid & Sold
            order.Status = OrderStatus.Paid;
            order.Product.IsSold = true;
            order.Product.IsArchived = false;
            await _db.SaveChangesAsync();

            // Fetch past orders AFTER saving this one
            var pastOrders = await _db.Orders
                .Include(o => o.Product)
                .Include(o => o.Payment)
                .Where(o => o.BuyerId == order.BuyerId && o.Status == OrderStatus.Paid)
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .ToListAsync();

            // Build the ViewModel INCLUDING Past Purchases
            var vm = new CheckoutSuccessViewModel
            {
                OrderId = order.OrderId,
                PurchaseDate = order.CreatedAt,
                AmountPaid = order.Payment?.Amount ?? order.Product.Price,
                PaymentStatus = order.Payment?.PaymentStatus ?? "Paid",
                ItemName = order.Product.Name,
                Description = order.Product.Description,
                ImageUrl = order.Product.ImageUrl,
                SellerName = order.Product.Seller?.Username ?? "Unknown Seller",
                SellerPhone = order.Product.SellerPhone,
                Street = order.Product.PickupAddress,
                Latitude = order.Product.Latitude,
                Longitude = order.Product.Longitude,
                PastPurchases = pastOrders.Select(o => new PastPurchaseItem
                {
                    OrderId = o.OrderId,
                    ItemName = o.Product?.Name ?? "Unknown",
                    AmountPaid = o.Payment?.Amount ?? o.Product?.Price ?? 0,
                    PurchaseDate = o.CreatedAt,
                    Status = "Paid"
                }).ToList()
            };

            return View(vm);
        }

        /// <summary>
        /// Displays a simple cancellation view if the user exits Stripe Checkout.
        /// </summary>
        public IActionResult Cancel(int orderId)
        {
            return View(orderId);
        }
    }
}
