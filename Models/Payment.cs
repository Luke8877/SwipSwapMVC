using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SwipSwapMVC.Models;

namespace SwipSwapMarketplace.Models
{
    /// <summary>
    /// Represents a payment record linked to an order.
    /// Used to track transactions processed through Stripe or other payment providers.
    /// </summary>
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        [Required]
        public string Provider { get; set; } = "Stripe";

        public string? ProviderPaymentId { get; set; }

        [Required]
        public string PaymentStatus { get; set; } = "Pending";

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public Order? Order { get; set; }
    }
}