using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using SwipSwapMVC.Data;
using SwipSwapMVC.Models;

namespace SwipSwapMVC.Controllers
{
    /// <summary>
    /// Receives and processes webhook events sent by Stripe.
    /// This endpoint is called server-to-server and finalizes orders after a successful payment.
    /// </summary>
    [Route("stripe/webhook")]
    [ApiController]
    public class StripeWebhookController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public StripeWebhookController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        /// <summary>
        /// Handles incoming webhook events from Stripe.
        /// Verifies the signature, checks event type, and updates Order + Payment state.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            // Read raw JSON payload from Stripe
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var webhookSecret = _config["Stripe:WebhookSecret"];
            Event stripeEvent;

            // Validate the event using Stripe's provided signature header
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    webhookSecret
                );
            }
            catch
            {
                // Signature mismatch or invalid payload
                return BadRequest();
            }

            // Only care about completed checkout sessions.
            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;

                // Retrieve the orderId that is embedded during session creation
                if (session.Metadata.TryGetValue("orderId", out string orderIdStr) &&
                    int.TryParse(orderIdStr, out int orderId))
                {
                    // Load product relationship using Include so IsSold can update
                    var order = await _db.Orders
                        .Include(o => o.Product)
                        .FirstOrDefaultAsync(o => o.OrderId == orderId);

                    var payment = await _db.Payments
                        .FirstOrDefaultAsync(p => p.OrderId == orderId);

                    // Defensive checks ensure we only update valid records
                    if (order != null && payment != null)
                    {
                        // Mark both order and payment as completed
                        order.Status = OrderStatus.Paid;
                        payment.PaymentStatus = "Succeeded";
                        payment.PaidAt = DateTime.UtcNow;

                        // Mark product unavailable so other users can't purchase it
                        if (order.Product != null)
                            order.Product.IsSold = true;

                        await _db.SaveChangesAsync();
                    }
                }
            }

            // Always return 200 so Stripe knows the event was received successfully
            return Ok();
        }
    }
}
