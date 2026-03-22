using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models;

namespace HikeCycle.Mvc.Models.db
{
    public class HikeCycledbContext : DbContext
    {
        public HikeCycledbContext(DbContextOptions<HikeCycledbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<ProductImage>().ToTable("product_images", t => t.ExcludeFromMigrations());
        }
    }
}
