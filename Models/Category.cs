using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SwipSwapMarketplace.Models
{
    /// <summary>
    /// Represents a product category within the marketplace.
    /// Categories help organize listings and make browsing easier for users.
    /// </summary>
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // Navigation Property
        public ICollection<Product>? Products { get; set; }  // Products associated with this category
    }
}