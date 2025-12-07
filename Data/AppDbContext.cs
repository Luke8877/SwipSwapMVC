using Microsoft.EntityFrameworkCore;
using SwipSwapMVC.Models;

namespace SwipSwapMVC.Data
{
    /// <summary>
    /// Primary EF Core DbContext for the SwipSwap marketplace.
    /// Defines database sets, relationship rules, and development seed data.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Address> Addresses { get; set; }

        /// <summary>
        /// Configures entity relationships, delete behaviors, and seed data.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --------------------------------------------------------------------
            // RELATIONSHIP CONFIGURATION
            // --------------------------------------------------------------------

            // User → Orders (Buyer Relationship)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Buyer)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → Products (Seller Relationship)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Seller)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order ↔ Payment (One-to-One)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Stripe-friendly: allow multiple orders per product
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.ProductId)
                .IsUnique(false);

            // --------------------------------------------------------------------
            // SEED DATA FOR DEVELOPMENT / DEMO
            // --------------------------------------------------------------------

            // Seed Demo User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "Demo User",
                    Email = "demo@example.com",
                    PasswordHash = "Password123!", // placeholder for now
                    PhoneNumber = "0000000000",
                    DateCreated = DateTime.UtcNow
                }
            );

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Electronics" },
                new Category { CategoryId = 2, Name = "Clothing" },
                new Category { CategoryId = 3, Name = "Home & Garden" },
                new Category { CategoryId = 4, Name = "Sports" },
                new Category { CategoryId = 5, Name = "Health and Beauty" },
                new Category { CategoryId = 6, Name = "Books" }
            );

            // Seed Products for Marketplace UI + Stripe Testing
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    Name = "iPhone 14 Pro",
                    Description = "256GB — Deep Purple — excellent condition",
                    Price = 1199.99m,
                    ImageUrl = "/uploads/iphone14.jpg",
                    CategoryId = 1,
                    SellerId = 1,
                    PickupAddress = "8882 170 St NW, Edmonton, AB",
                    Latitude = 53.5232,
                    Longitude = -113.6247,
                    SellerPhone = "780-111-1111",
                    IsSold = false,
                    DatePosted = DateTime.UtcNow
                },
                new Product
                {
                    ProductId = 2,
                    Name = "Gaming Laptop",
                    Description = "RTX 3070 — 16GB RAM — 1TB SSD",
                    Price = 1599.00m,
                    ImageUrl = "/uploads/laptop.jpg",
                    CategoryId = 1,
                    SellerId = 1,
                    PickupAddress = "7005 Gateway Blvd NW, Edmonton, AB",
                    Latitude = 53.4890,
                    Longitude = -113.4987,
                    SellerPhone = "780-222-2222",
                    IsSold = false,
                    DatePosted = DateTime.UtcNow
                },
                new Product
                {
                    ProductId = 3,
                    Name = "Mountain Bike",
                    Description = "Aluminum frame — good condition",
                    Price = 450.00m,
                    ImageUrl = "/uploads/bike.jpg",
                    CategoryId = 4,
                    SellerId = 1,
                    PickupAddress = "1000 Taylor Dr, Red Deer, AB",
                    Latitude = 52.2936,
                    Longitude = -113.8187,
                    SellerPhone = "780-333-3333",
                    IsSold = false,
                    DatePosted = DateTime.UtcNow
                }

            );
        }
      }
    }

