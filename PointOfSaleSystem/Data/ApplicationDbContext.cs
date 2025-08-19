using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Models.Auth;
using PointOfSaleSystem.Models.Inventory;
using PointOfSaleSystem.Models.Printing;
using PointOfSaleSystem.Models.Reports;
using PointOfSaleSystem.Models.Sales;
using PointOfSaleSystem.Models.Settings;
using PointOfSaleSystem.Models.Suppliers;

namespace PointOfSaleSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        // Auth
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<LoginAttemptLog> LoginAttemptLogs { get; set; }

        //Inventory
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<BadOrder> BadOrders { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<ProductUnitConversion> ProductUnitConversions { get; set; }
        public DbSet<StockAdjustment> StockAdjustments { get; set; }
        public DbSet<StockReceive> StockReceives { get; set; }

        //Printing
        public DbSet<BackupLog> BackupLogs { get; set; }
        public DbSet<InvoiceCounter> InvoiceCounters { get; set; }
        public DbSet<ReprintLog> ReprintLogs { get; set; }
        public DbSet<SerialNumberTracker> SerialNumberTracker { get; set; }

        //Reports
        public DbSet<CashierSalesSummary> CashierSalesSummaries { get; set; }
        public DbSet<DailySalesSummary> DailySalesSummaries { get; set; }
        public DbSet<InventoryValuationSnapshot> InventoryValuationSnapshots { get; set; }
        public DbSet<ProductSalesHistory> ProductSalesHistories { get; set; }
        public DbSet<TopSellingProductLog> TopSellingProductLogs { get; set; }

        //Sales
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ReceiptLog> ReceiptLogs { get; set; }
        public DbSet<ReturnedItem> ReturnedItems { get; set; }
        public DbSet<ReturnTransaction> ReturnTransactions { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<VoidTransaction> VoidTransactions { get; set; }
        public DbSet<SaleAuditTrail> SaleAuditTrails { get; set; }

        //Settings
        public DbSet<BusinessProfile> BusinessProfiles { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<DiscountSetting> DiscountSettings { get; set; }
        public DbSet<ReportExportLog> ReportExportLogs { get; set; }
        public DbSet<ReceiptSetting> ReceiptSettings { get; set; }
        public DbSet<VatSetting> VatSettings { get; set; }
        public DbSet<XReading> XReadings { get; set; }
        public DbSet<ZReading> ZReadings { get; set; }

        //Suppliers
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<ReceivedStock> ReceivedStocks { get; set; }
        public DbSet<StockReceiveItem> StockReceiveItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            });

            // User sessions: cascade deleting a user will delete sessions (keep or change as desired).
            builder.Entity<UserSession>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // SystemLog -> User: keep as Restrict (prevent user deletion if logs exist)
            builder.Entity<SystemLog>()
                .HasOne(log => log.User)
                .WithMany(user => user.Logs)
                .HasForeignKey(log => log.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product relations
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasIndex(p => p.Barcode)
                .IsUnique()
                .HasFilter("[Barcode] IS NOT NULL");

            builder.Entity<Product>()
                .HasOne(p => p.Unit)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            // Purchase order / items
            builder.Entity<PurchaseOrderItem>()
                .HasOne(i => i.PurchaseOrder)
                .WithMany(po => po.Items)
                .HasForeignKey(i => i.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PurchaseOrderItem>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PurchaseOrderItem>()
                .HasOne(i => i.Unit)
                .WithMany()
                .HasForeignKey(i => i.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Product)
                .WithMany()
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unit conversions
            builder.Entity<ProductUnitConversion>()
                .HasOne(puc => puc.FromUnit)
                .WithMany(unit => unit.FromConversions)
                .HasForeignKey(puc => puc.FromUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProductUnitConversion>()
                .HasOne(puc => puc.ToUnit)
                .WithMany(unit => unit.ToConversions)
                .HasForeignKey(puc => puc.ToUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProductUnitConversion>()
                .HasOne(puc => puc.Product)
                .WithMany()
                .HasForeignKey(puc => puc.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Report export log
            builder.Entity<ReportExportLog>()
                .HasOne(r => r.ExportedByUser)
                .WithMany()
                .HasForeignKey(r => r.ExportedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Receipt/Reprint logs: do not cascade-delete Sales
            builder.Entity<ReceiptLog>()
                .HasOne(r => r.Sale)
                .WithMany()
                .HasForeignKey(r => r.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReceiptLog>()
                .HasOne(r => r.PrintedBy)
                .WithMany()
                .HasForeignKey(r => r.PrintedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReprintLog>()
                .HasOne(r => r.Sale)
                .WithMany()
                .HasForeignKey(r => r.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReprintLog>()
                .HasOne(r => r.ReprintedByUser)
                .WithMany()
                .HasForeignKey(r => r.ReprintedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ReturnTransaction -> Sale: restrict deleting sale (don't cascade)
            builder.Entity<ReturnTransaction>()
                .HasOne(r => r.OriginalSale)
                .WithMany()
                .HasForeignKey(r => r.OriginalSaleId)
                .OnDelete(DeleteBehavior.Restrict);

            // ReturnTransaction -> ReturnedBy user
            builder.Entity<ReturnTransaction>()
                .HasOne(r => r.ReturnedBy)
                .WithMany()
                .HasForeignKey(r => r.ReturnedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // SaleItem relations: do not cascade delete Sale -> SaleItem
            builder.Entity<SaleItem>()
                .HasOne(si => si.Sale)
                .WithMany(s => s.SaleItems)
                .HasForeignKey(si => si.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SaleItem>()
                .HasOne(si => si.Unit)
                .WithMany()
                .HasForeignKey(si => si.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SaleItem>()
                .HasOne(si => si.Product)
                .WithMany()
                .HasForeignKey(si => si.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // VoidTransaction -> Sale: do not cascade-delete
            builder.Entity<VoidTransaction>()
                .HasOne(v => v.Sale)
                .WithMany()
                .HasForeignKey(v => v.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            // VoidTransaction -> user references: restrict
            builder.Entity<VoidTransaction>()
                .HasOne(v => v.VoidedBy)
                .WithMany()
                .HasForeignKey(v => v.VoidedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VoidTransaction>()
                .HasOne(v => v.OriginalCashier)
                .WithMany()
                .HasForeignKey(v => v.OriginalCashierUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VoidTransaction>()
                .HasOne(v => v.ApprovalUser)
                .WithMany()
                .HasForeignKey(v => v.ApprovalUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Discount applied/approved by user -> restrict
            builder.Entity<Discount>()
                .HasOne(d => d.AppliedByUser)
                .WithMany()
                .HasForeignKey(d => d.AppliedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Discount>()
                .HasOne(d => d.ApprovedByUser)
                .WithMany()
                .HasForeignKey(d => d.ApprovedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment -> user trace: restrict
            builder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // InventoryTransaction -> PerformedByUser: restrict
            builder.Entity<InventoryTransaction>()
                .HasOne(t => t.PerformedByUser)
                .WithMany()
                .HasForeignKey(t => t.PerformedById)
                .OnDelete(DeleteBehavior.Restrict);

            // BadOrder -> ReportedByUser: restrict
            builder.Entity<BadOrder>()
                .HasOne(b => b.ReportedByUser)
                .WithMany()
                .HasForeignKey(b => b.ReportedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // StockAdjustment -> AdjustedByUser: restrict
            builder.Entity<StockAdjustment>()
                .HasOne(sa => sa.AdjustedByUser)
                .WithMany()
                .HasForeignKey(sa => sa.AdjustedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // StockReceive -> ReceivedByUser: restrict
            builder.Entity<StockReceive>()
                .HasOne(sr => sr.ReceivedByUser)
                .WithMany()
                .HasForeignKey(sr => sr.ReceivedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // StockReceiveItem -> InventoryTransaction: restrict
            builder.Entity<StockReceiveItem>()
                .HasOne(sri => sri.InventoryTransaction)
                .WithMany()
                .HasForeignKey(sri => sri.InventoryTransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            // BadOrder -> InventoryTransaction: restrict
            builder.Entity<BadOrder>()
                .HasOne(b => b.InventoryTransaction)
                .WithMany()
                .HasForeignKey(b => b.InventoryTransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            // StockAdjustment -> InventoryTransaction: restrict
            builder.Entity<StockAdjustment>()
                .HasOne(sa => sa.InventoryTransaction)
                .WithMany()
                .HasForeignKey(sa => sa.InventoryTransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----------------- SALE AUDIT TRAIL -----------------
            builder.Entity<SaleAuditTrail>()
                .HasOne(a => a.Sale)
                .WithMany()
                .HasForeignKey(a => a.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SaleAuditTrail>()
                .HasOne(a => a.PerformedBy)
                .WithMany()
                .HasForeignKey(a => a.PerformedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
