using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SwipSwapMVC.Models;

namespace SwipSwapMVC.Models
{
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

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(Seller))]
        public int SellerId { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsSold { get; set; } = false;

        public bool IsArchived { get; set; } = false;   

        public DateTime DatePosted { get; set; } = DateTime.Now;

        public Category? Category { get; set; }

        public User? Seller { get; set; }   

        public Order? Order { get; set; }

        public string? PickupAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? SellerPhone { get; set; }
    }
}
