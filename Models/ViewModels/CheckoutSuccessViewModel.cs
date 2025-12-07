namespace SwipSwapMVC.Models.ViewModels
{
    public class CheckoutSuccessViewModel
    {
        // Order Details
        public int OrderId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;

        // Product Details
        public string ItemName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        // Seller Details
        public string SellerName { get; set; } = string.Empty;
        public string? SellerPhone { get; set; }

        // Address Details
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string FullAddress
        {
            get
            {
                var parts = new[]
                {
            Street,
            City,
            Province,
            PostalCode
        }
                .Where(p => !string.IsNullOrWhiteSpace(p));

                return string.Join(", ", parts);
            }
        }

        public List<PastPurchaseItem> PastPurchases { get; set; } = new();

        public class PastPurchaseItem
        {
            public int OrderId { get; set; }
            public string ItemName { get; set; } = string.Empty;
            public decimal AmountPaid { get; set; }
            public DateTime PurchaseDate { get; set; }
            public string Status { get; set; } = "Paid";
        }


    }
}
