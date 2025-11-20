using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwipSwapMarketplace.Models
{
    /// <summary>
    /// Represents a purchase transaction between a buyer and seller.
    /// Each order links a product to its purchaser and tracks payment and fulfillment status.
    /// </summary>
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [ForeignKey(nameof(Buyer))]
        public int BuyerId { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public string DeliveryType { get; set; } = "Pickup"; // Options: Pickup or Delivery

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled, etc.

        // Navigation Properties
        public User? Buyer { get; set; }       // The user who placed the order
        public Product? Product { get; set; }  // The purchased product
        public Payment? Payment { get; set; }  // Related payment record, if applicable
    }
}