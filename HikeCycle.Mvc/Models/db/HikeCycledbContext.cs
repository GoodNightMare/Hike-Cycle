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

        public DbSet<Reviews> Reviews { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        // เพิ่มเข้าไปใน class HikeCycledbContext
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionCondition> PromotionConditions { get; set; }
        public DbSet<PromotionBenefit> PromotionBenefits { get; set; }

        public DbSet<RecommendedRoute> RecommendedRoutes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<ProductImage>().ToTable("product_images", t => t.ExcludeFromMigrations());

            modelBuilder.Entity<Reviews>(entity =>
            {
                entity.ToTable("reviews"); // ชื่อตารางใน Database

                entity.Property(e => e.Comment)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");
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
                    .UseCollation("utf8mb3_general_ci");

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");

                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");
            });

            // Configuration สำหรับ Promotion, PromotionCondition, PromotionBenefit
            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.ToTable("promotions");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");

                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<PromotionCondition>(entity =>
            {
                entity.ToTable("promotion_conditions");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ConditionKey)
                    .HasMaxLength(100)
                    .HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");

                entity.Property(e => e.ConditionValue)
                    .HasMaxLength(100)
                    .HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");
            });

            modelBuilder.Entity<PromotionBenefit>(entity =>
            {
                entity.ToTable("promotion_benefits");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.BenefitKey)
                    .HasMaxLength(100)
                    .HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");

                entity.Property(e => e.BenefitValue)
                    .HasMaxLength(100)
                    .HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");
            });

            modelBuilder.Entity<RecommendedRoute>(entity =>
            {
                entity.ToTable("recommended_routes");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");

                entity.Property(e => e.Province)
                    .HasMaxLength(100)
                    .HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");

                entity.Property(e => e.Duration)
                    .HasMaxLength(50)
                    .HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");

                entity.Property(e => e.Distance)
                    .HasMaxLength(50)
                    .HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");

                entity.Property(e => e.Level)
                    .HasMaxLength(50)
                    .HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");

                entity.Property(e => e.Highlight)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.Suitable)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");
            });

        }
    }
}
