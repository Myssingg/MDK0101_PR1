using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PR1.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Basket> Baskets { get; set; }

    public virtual DbSet<Bouquet> Bouquets { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=0101_PR1;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("C");

        modelBuilder.Entity<Basket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("baskets_pkey");

            entity.ToTable("baskets");

            entity.HasIndex(e => new { e.UserId, e.BouquetId }, "basketsUserBouquetUnique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BouquetId).HasColumnName("bouquetId");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Bouquet).WithMany(p => p.Baskets)
                .HasForeignKey(d => d.BouquetId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("baskets_bouquetId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Baskets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("baskets_userId_fkey");
        });

        modelBuilder.Entity<Bouquet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bouquets_pkey");

            entity.ToTable("bouquets");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(200)
                .HasColumnName("imagePath");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.HasIndex(e => e.UserId, "ordersUserIdIdx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeliveryApartment).HasColumnName("deliveryApartment");
            entity.Property(e => e.DeliveryHouse)
                .HasMaxLength(20)
                .HasColumnName("deliveryHouse");
            entity.Property(e => e.DeliveryStreet)
                .HasMaxLength(150)
                .HasColumnName("deliveryStreet");
            entity.Property(e => e.DeliveryTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deliveryTime");
            entity.Property(e => e.TotalCost)
                .HasPrecision(10, 2)
                .HasColumnName("totalCost");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("orders_userId_fkey");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orderItems_pkey");

            entity.ToTable("orderItems");

            entity.HasIndex(e => new { e.OrderId, e.BouquetId }, "orderItemsOrderBouquetUnique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BouquetId).HasColumnName("bouquetId");
            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unitPrice");

            entity.HasOne(d => d.Bouquet).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.BouquetId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("orderItems_bouquetId_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("orderItems_orderId_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Login, "usersLoginUnique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Apartment).HasColumnName("apartment");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .HasColumnName("fullName");
            entity.Property(e => e.House)
                .HasMaxLength(20)
                .HasColumnName("house");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Street)
                .HasMaxLength(150)
                .HasColumnName("street");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
