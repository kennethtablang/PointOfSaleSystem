using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Models.Auth;
using PointOfSaleSystem.Models.Inventory;

namespace PointOfSaleSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        // Auth-related tables
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<LoginAttemptLog> LoginAttemptLogs { get; set; }

        //Inventory and Sales Related
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Unit> Units { get; set; }

        // TODO: Add your future models like Products, Categories, Sales, etc.

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Optional: Fluent API or custom table names

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            });

            //User Session relationships
            builder.Entity<UserSession>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //System Log relationships
            builder.Entity<SystemLog>()
                .HasOne(log => log.User)
                .WithMany(user => user.Logs)
                .HasForeignKey(log => log.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            //Product relationships
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete by default
        }
    }
}
