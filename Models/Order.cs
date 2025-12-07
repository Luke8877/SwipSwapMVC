using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwipSwapMVC.Models
{
    /// <summary>
    /// Represents a purchase transaction between a buyer and seller.
    /// </summary>
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [ForeignKey(nameof(Buyer))]
        public int? BuyerId { get; set; }
        public User? Buyer { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }   
        public Product? Product { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public Payment? Payment { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Paid,
        Cancelled,
        Failed
    }
}
