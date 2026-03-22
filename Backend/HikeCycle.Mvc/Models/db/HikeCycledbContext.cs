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

        public DbSet<Review> Reviews { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }


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

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("reviews"); // ชื่อตารางใน Database

                entity.Property(e => e.Comment)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("user_profiles");
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .HasCharSet("utf8mb3")
                    .HasCollation("utf8mb3_general_ci");

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .HasCharSet("utf8mb3")
                    .HasCollation("utf8mb3_general_ci");

                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .HasCharSet("utf8mb3")
                    .HasCollation("utf8mb3_general_ci");
            });
        }
    }
}
