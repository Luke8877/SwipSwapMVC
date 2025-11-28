using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SwipSwapMVC.Models;

namespace SwipSwapMarketplace.Models
{
    /// <summary>
    /// Represents a product listed for sale in the SwipSwap marketplace.
    /// Each product is associated with a seller, category, and may be linked to an order once sold.
    /// </summary>
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [ForeignKey("User")]
        public int SellerId { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsSold { get; set; } = false;

        public DateTime DatePosted { get; set; } = DateTime.Now;

        // Navigation Properties
        public Category? Category { get; set; }   // Category the product belongs to
        public User? User { get; set; }           // Seller who listed the product
        public Order? Order { get; set; }         // Linked order if the product has been sold
    }
}