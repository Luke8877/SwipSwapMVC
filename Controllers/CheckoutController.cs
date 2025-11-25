using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipSwapMVC.Data;
using SwipSwapMarketplace.Models;
using Stripe.Checkout;

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
            // TODO: Replace with authenticated user ID when identity is implemented.
            int buyerId = 1;

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
        public IActionResult Success(int orderId)
        {
            return View(orderId);
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
