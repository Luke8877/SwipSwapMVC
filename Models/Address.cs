using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwipSwapMarketplace.Models
{
    /// <summary>
    /// Represents a physical address associated with a user account.
    /// Can be used for delivery, pickup, or user profile information.
    /// </summary>
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [Required]
        public string Street { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Province { get; set; } = string.Empty;

        [Required]
        public string PostalCode { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public User? User { get; set; }  
    }
}