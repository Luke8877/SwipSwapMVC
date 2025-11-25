using Microsoft.EntityFrameworkCore;
using SwipSwapMarketplace.Models;
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

            // User → Orders (Buyer relationship)
            // Prevents deleting a user from automatically deleting their order history.
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Buyer)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → Products (Seller relationship)
            // Marketplace logic: if a seller profile is removed, their listings should also be removed.
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Seller)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order ↔ Payment (One-to-One)
            // Each order generates exactly one payment record.
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.ProductId)
                .IsUnique(false);

            // --------------------------------------------------------------------
            // SEED DATA FOR DEVELOPMENT
            // --------------------------------------------------------------------
            // These records allow Stripe payment flow to work during early development
            // without requiring a full authentication system or listing management UI.

            // Seed a test user (acts as both buyer and seller)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "TestUser",
                    Email = "test@test.com",
                    PasswordHash = "fakehash",         // placeholder: not used for login yet
                    PhoneNumber = "0000000000",
                    DateCreated = DateTime.UtcNow
                }
            );

            // Default category so products have a valid FK
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    Name = "General"
                }
            );

            // Seed one product so CheckoutController has something to purchase
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    Name = "Test Product",
                    Description = "Seeded test product",
                    Price = 10.00m,
                    CategoryId = 1,
                    SellerId = 1,
                    IsSold = false,
                    DatePosted = DateTime.UtcNow,
                    ImageUrl = null
                }
            );
        }
    }
}
