using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InventoryOrderSystem.Models;
using System.Reflection.Emit;

namespace InventoryOrderSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
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

            product.HasIndex(p=> p.SKU).IsUnique();
            product.Property(p=> p.SKU).HasMaxLength(50);
            product.Property(p=> p.Price).HasColumnType("decimal(18,2)");
            product.HasOne(p=> p.Category)
                   .WithMany(oi=> oi.Products)
                   .HasForeignKey(p=> p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            orderItem.Property(oi=> oi.UnitPrice).HasColumnType("decimal(18,2)");
            orderItem.HasOne(oi=> oi.Product)
                     .WithMany(p=> p.OrderItems)
                     .HasForeignKey(oi=> oi.ProductId)
                     .OnDelete(DeleteBehavior.Restrict);

            order.Property(o=> o.TotalAmount).HasColumnType("decimal(18,2)");
        }

    }
}