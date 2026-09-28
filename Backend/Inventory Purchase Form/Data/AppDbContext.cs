using Inventory_Purchase_Form.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Purchase_Form.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Tax> Taxes => Set<Tax>();
        public DbSet<ProductTax> ProductTaxes => Set<ProductTax>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Purchase> Purchases => Set<Purchase>();
        public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
        public DbSet<PurchaseItemTax> PurchaseItemTaxes => Set<PurchaseItemTax>();
        public DbSet<Inventory> Inventories => Set<Inventory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Product ──
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(p => p.ProductCode).IsUnique();
                entity.Property(p => p.CostPrice).HasColumnType("decimal(18,2)");
                entity.Property(p => p.SellingPrice).HasColumnType("decimal(18,2)");
            });

            // ── Tax ──
            modelBuilder.Entity<Tax>(entity =>
            {
                entity.Property(t => t.Percentage).HasColumnType("decimal(18,4)");
                // Global query filter to exclude soft-deleted taxes by default
                entity.HasQueryFilter(t => !t.IsDeleted);
            });

            // ── ProductTax ──
            modelBuilder.Entity<ProductTax>(entity =>
            {
                entity.HasIndex(pt => new { pt.ProductId, pt.TaxId }).IsUnique();

                entity.HasOne(pt => pt.Product)
                    .WithMany(p => p.ProductTaxes)
                    .HasForeignKey(pt => pt.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pt => pt.Tax)
                    .WithMany(t => t.ProductTaxes)
                    .HasForeignKey(pt => pt.TaxId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Supplier ──
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasIndex(s => s.SupplierName);
            });

            // ── Purchase ──
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.Property(p => p.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(p => p.TotalTax).HasColumnType("decimal(18,2)");
                entity.Property(p => p.NetAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(p => p.Supplier)
                    .WithMany(s => s.Purchases)
                    .HasForeignKey(p => p.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ── PurchaseItem ──
            modelBuilder.Entity<PurchaseItem>(entity =>
            {
                entity.Property(pi => pi.Quantity).HasColumnType("decimal(18,4)");
                entity.Property(pi => pi.Rate).HasColumnType("decimal(18,4)");
                entity.Property(pi => pi.DiscountPercent).HasColumnType("decimal(18,2)");
                entity.Property(pi => pi.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(pi => pi.LineTotal).HasColumnType("decimal(18,2)");
                entity.Property(pi => pi.TaxAmount).HasColumnType("decimal(18,2)");
                entity.Property(pi => pi.NetAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(pi => pi.Purchase)
                    .WithMany(p => p.PurchaseItems)
                    .HasForeignKey(pi => pi.PurchaseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pi => pi.Product)
                    .WithMany(p => p.PurchaseItems)
                    .HasForeignKey(pi => pi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ── PurchaseItemTax ──
            modelBuilder.Entity<PurchaseItemTax>(entity =>
            {
                entity.Property(pit => pit.TaxPercentage).HasColumnType("decimal(18,4)");
                entity.Property(pit => pit.TaxAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(pit => pit.PurchaseItem)
                    .WithMany(pi => pi.PurchaseItemTaxes)
                    .HasForeignKey(pit => pit.PurchaseItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pit => pit.Tax)
                    .WithMany(t => t.PurchaseItemTaxes)
                    .HasForeignKey(pit => pit.TaxId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Inventory ──
            modelBuilder.Entity<Inventory>(entity =>
            {
                // Unique composite index: one stock record per product + lot
                entity.HasIndex(i => new { i.ProductId, i.LotNumber }).IsUnique();
                entity.Property(i => i.Quantity).HasColumnType("decimal(18,4)");

                entity.HasOne(i => i.Product)
                    .WithMany(p => p.Inventories)
                    .HasForeignKey(i => i.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
