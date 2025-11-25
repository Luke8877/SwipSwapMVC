using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SwipSwapMVC.Models;

namespace SwipSwapMarketplace.Models
{
    /// <summary>
    /// Represents an individual user within the SwipSwap marketplace.
    /// Users can act as buyers or sellers and may have associated listings, orders, and addresses.
    /// </summary>
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public ICollection<Product>? Products { get; set; }   // Listings created by the user 
        public ICollection<Order>? Orders { get; set; }       // Orders placed by the user 
        public ICollection<Address>? Addresses { get; set; }  // Saved addresses linked to the user
    }
}