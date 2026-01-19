using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Models;
using Restaurant.Infrastructure.Identity;

namespace Restaurant.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppilcationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Chef> Chefs { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Customer>()
            .HasOne<AppilcationUser>()
            .WithOne(c => c.Customer).HasForeignKey<Customer>(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Customer>()
            .HasIndex(c => c.UserId)
            .IsUnique();

            builder.Entity<Chef>()
           .HasOne<AppilcationUser>()
           .WithOne(c => c.Chef).HasForeignKey<Chef>(c => c.UserId)
           .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Chef>()
           .HasIndex(c => c.UserId)
           .IsUnique();

            builder.Entity<Delivery>()
            .HasOne<AppilcationUser>()
            .WithOne(c => c.Delivery).HasForeignKey<Delivery>(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Delivery>()
            .HasIndex(c => c.UserId)
            .IsUnique();

            builder.Entity<Order>()
            .HasOne(o => o.Payment)
          .WithOne(p => p.Order)
           .HasForeignKey<Payment>(p => p.OrderId)
           .OnDelete(DeleteBehavior.Cascade)
           .IsRequired(false);

        }

    }
}
