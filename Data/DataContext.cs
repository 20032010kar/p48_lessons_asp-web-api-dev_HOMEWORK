using System;
using Microsoft.EntityFrameworkCore;
using p48_lessons_asp.Data.Entity;

namespace p48_lessons_asp.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options) { }

    public DbSet<ProductImage> ProductImages { get; set; }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .ToTable(t => t.HasCheckConstraint("CK_Product_Price_Positive", "Price >= 0"));
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);
        modelBuilder.Entity<Product>()
           .HasIndex(p => p.Slug)
           .IsUnique();
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = Guid.Parse("a2b3c4d5-e6f7-8a9b-0c1d-2e3f4a5b6c7d"),
                Title="Electronics"
            }
        );
        modelBuilder.Entity<Category>()
        .HasIndex(c => c.Slug)
        .IsUnique();
        modelBuilder.Entity<ProductImage>()
        .HasOne(pi => pi.Product)
        .WithMany(p => p.Images)
        .HasForeignKey(pi => pi.ProductId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}
