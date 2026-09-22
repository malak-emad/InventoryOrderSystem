using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InventoryOrderSystem.Models;

namespace InventoryOrderSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        // Database tables
        public DbSet<Product> Products {get; set; }
        public DbSet<Category> Categories {get; set; }
        public DbSet<Order> Orders {get; set; }
        public DbSet<OrderItem> OrderItems {get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var product = modelBuilder.Entity<Product>();
            var orderItem = modelBuilder.Entity<OrderItem>();
            var order = modelBuilder.Entity<Order>();

            // Product constraints and Category relationship
            product.HasIndex(p=> p.SKU).IsUnique();
            product.Property(p=> p.SKU).HasMaxLength(50);
            product.Property(p=> p.Price).HasColumnType("decimal(18,2)");
            product.HasOne(p=> p.Category)
                   .WithMany(oi=> oi.Products)
                   .HasForeignKey(p=> p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // OrderItem constraints and Product relationship
            orderItem.Property(oi=> oi.UnitPrice).HasColumnType("decimal(18,2)");
            orderItem.HasOne(oi=> oi.Product)
                     .WithMany(p=> p.OrderItems)
                     .HasForeignKey(oi=> oi.ProductId)
                     .OnDelete(DeleteBehavior.Restrict);

            // Order total precision
            order.Property(o=> o.TotalAmount).HasColumnType("decimal(18,2)");
        }

    }
}