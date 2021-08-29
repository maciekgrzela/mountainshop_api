using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context
{
    public class DataContext : IdentityDbContext<User, IdentityRole, string>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductsProperty> ProductsProperties { get; set; }
        public DbSet<ProductsPropertyValue> ProductsPropertyValues { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<ProductsOrders> ProductsOrders { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ContactRequest> ContactRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<Complaint>()
                .Property(p => p.Number)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Order>()
                .Property(p => p.Number)
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Producer>()
                .HasIndex(p => p.Name)
                .IsUnique();
            
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<ProductsProperty>()
                .HasIndex(p => p.Name)
                .IsUnique();
        }
    }
}