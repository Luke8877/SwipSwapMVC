using Microsoft.EntityFrameworkCore;
using SwipSwapMVC.Models;
using SwipSwapMVC.Models;

namespace SwipSwapMVC.Data
{
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
               

            // ---------------------------
            // SEED CATEGORIES
            // ---------------------------
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Electronics" },
                new Category { CategoryId = 2, Name = "Vehicles" },
                new Category { CategoryId = 3, Name = "Sports" },
                new Category { CategoryId = 4, Name = "Home & Garden" }
            );

            // ---------------------------
            // SEED PRODUCTS
            // ---------------------------
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    Name = "iPhone 14 Pro",
                    Description = "256GB — Deep Purple — excellent condition",
                    Price = 1199.99m,
                    ImageUrl = "/uploads/iphone14.jpg",
                    CategoryId = 1,   // Electronics
                    SellerId = 1,     // Demo user
                    IsSold = false,
                    DatePosted = DateTime.Now
                },
                new Product
                {
                    ProductId = 2,
                    Name = "Gaming Laptop",
                    Description = "RTX 3070 — 16GB RAM — 1TB SSD",
                    Price = 1599.00m,
                    ImageUrl = "/uploads/laptop.jpg",
                    CategoryId = 1,   // Electronics
                    SellerId = 1,
                    IsSold = false,
                    DatePosted = DateTime.Now
                },
                new Product
                {
                    ProductId = 3,
                    Name = "Mountain Bike",
                    Description = "Aluminum frame — good condition",
                    Price = 450.00m,
                    ImageUrl = "/uploads/bike.jpg",
                    CategoryId = 3,   // Sports
                    SellerId = 1,
                    IsSold = false,
                    DatePosted = DateTime.Now
                }
            );
        }
    }
}
