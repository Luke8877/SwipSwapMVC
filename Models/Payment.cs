using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwipSwapMarketplace.Models
{
    /// <summary>
    /// Represents a payment record for an order processed through Stripe.
    /// Stores checkout session details, final payment status, 
    /// and timestamps for tracking lifecycle events.
    /// </summary>
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        /// <summary>
        /// Foreign key linking this payment to its order.
        /// </summary>
        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        /// <summary>
        /// Navigation reference to the associated order.
        /// </summary>
        public Order? Order { get; set; }

        /// <summary>
        /// Payment provider used (Stripe, PayPal, etc). Defaults to Stripe.
        /// </summary>
        [Required]
        public string Provider { get; set; } = "Stripe";

        /// <summary>
        /// Stripe Checkout Session ID (e.g., cs_123).
        /// Used to verify and complete the transaction via webhook.
        /// </summary>
        public string? SessionId { get; set; }

        /// <summary>
        /// Stripe PaymentIntent ID (e.g., pi_123).
        /// Allows tracking and reconciliation of the payment.
        /// </summary>
        public string? PaymentIntentId { get; set; }

        /// <summary>
        /// Current status of the payment ("Pending", "Succeeded", "Failed").
        /// </summary>
        [Required]
        public string PaymentStatus { get; set; } = "Pending";

        /// <summary>
        /// Amount charged for the order.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Timestamp when the payment record was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the payment completed successfully.
        /// Null until payment succeeds.
        /// </summary>
        public DateTime? PaidAt { get; set; }
    }
}
