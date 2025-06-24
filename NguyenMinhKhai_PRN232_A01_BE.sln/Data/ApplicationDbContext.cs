using Microsoft.EntityFrameworkCore;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<News> NewsArticles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Account Configuration
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.AccountId);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.LastLoginDate).IsRequired(false);

                // Configure cascade delete for NewsArticles
                entity.HasMany(e => e.NewsArticles)
                    .WithOne(e => e.Account)
                    .HasForeignKey(e => e.AccountId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

                // Seed accounts
                var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                entity.HasData(
                    // Admin account
                    new Account
                    {
                        AccountId = 1,
                        Email = "admin@FUNewsManagementSystem.org",
                        Password = "@@abc123@@",
                        FullName = "System Administrator",
                        Role = 0, // Admin role
                        CreatedDate = seedDate
                    },
                    // Staff accounts
                    new Account
                    {
                        AccountId = 2,
                        Email = "staff1@FUNewsManagementSystem.org",
                        Password = "Staff123@@",
                        FullName = "Staff Member 1",
                        Role = 1, // Staff role
                        CreatedDate = seedDate
                    },
                    new Account
                    {
                        AccountId = 3,
                        Email = "staff2@FUNewsManagementSystem.org",
                        Password = "Staff123@@",
                        FullName = "Staff Member 2",
                        Role = 1, // Staff role
                        CreatedDate = seedDate
                    },
                    new Account
                    {
                        AccountId = 4,
                        Email = "staff3@FUNewsManagementSystem.org",
                        Password = "Staff123@@",
                        FullName = "Staff Member 3",
                        Role = 1, // Staff role
                        CreatedDate = seedDate
                    }
                );
            });

            // News Configuration
            modelBuilder.Entity<News>(entity =>
            {
                entity.HasKey(e => e.NewsId);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.ViewCount).HasDefaultValue(0);
                entity.Property(e => e.IsFeatured).HasDefaultValue(false);

                // Configure relationship with Category
                entity.HasOne(e => e.Category)
                    .WithMany(e => e.NewsArticles)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

                // Configure many-to-many relationship with Tags
                entity.HasMany(e => e.Tags)
                    .WithMany(e => e.NewsArticles)
                    .UsingEntity(
                        "NewsTags",
                        l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.Cascade),
                        r => r.HasOne(typeof(News)).WithMany().HasForeignKey("NewsId").OnDelete(DeleteBehavior.Cascade)
                    );

                // Seed news articles
                var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                entity.HasData(
                    new News
                    {
                        NewsId = 1,
                        Title = "Welcome to FU News Management System",
                        Content = "This is the first article in our news system. We are excited to bring you the latest updates and news.",
                        CategoryId = 1,
                        AccountId = 1,
                        CreatedDate = seedDate,
                        UpdatedDate = seedDate,
                        Status = 1,
                        ViewCount = 100,
                        IsFeatured = true
                    },
                    new News
                    {
                        NewsId = 2,
                        Title = "New Academic Programs Announced",
                        Content = "The university has announced several new academic programs for the upcoming semester.",
                        CategoryId = 2,
                        AccountId = 2,
                        CreatedDate = seedDate.AddDays(1),
                        UpdatedDate = seedDate.AddDays(1),
                        Status = 1,
                        ViewCount = 75,
                        IsFeatured = false
                    },
                    new News
                    {
                        NewsId = 3,
                        Title = "Student Success Stories",
                        Content = "Read about our outstanding students and their achievements in various fields.",
                        CategoryId = 3,
                        AccountId = 3,
                        CreatedDate = seedDate.AddDays(2),
                        UpdatedDate = seedDate.AddDays(2),
                        Status = 1,
                        ViewCount = 50,
                        IsFeatured = true
                    }
                );
            });

            // Category Configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.Order).HasDefaultValue(0);

                // Seed categories
                var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                entity.HasData(
                    new Category
                    {
                        CategoryId = 1,
                        Name = "General News",
                        Description = "General news and announcements",
                        Status = 1,
                        CreatedDate = seedDate,
                        Order = 1
                    },
                    new Category
                    {
                        CategoryId = 2,
                        Name = "Academic",
                        Description = "Academic news and updates",
                        Status = 1,
                        CreatedDate = seedDate,
                        Order = 2
                    },
                    new Category
                    {
                        CategoryId = 3,
                        Name = "Student Life",
                        Description = "News about student activities and achievements",
                        Status = 1,
                        CreatedDate = seedDate,
                        Order = 3
                    },
                    new Category
                    {
                        CategoryId = 4,
                        Name = "Events",
                        Description = "Upcoming events and activities",
                        Status = 1,
                        CreatedDate = seedDate,
                        Order = 4
                    }
                );
            });

            // Tag Configuration
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.TagId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.CreatedDate).IsRequired();

                // Seed tags
                var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                entity.HasData(
                    new Tag
                    {
                        TagId = 1,
                        Name = "Important",
                        Description = "Important announcements",
                        CreatedDate = seedDate
                    },
                    new Tag
                    {
                        TagId = 2,
                        Name = "Academic",
                        Description = "Academic related news",
                        CreatedDate = seedDate
                    },
                    new Tag
                    {
                        TagId = 3,
                        Name = "Student",
                        Description = "Student related news",
                        CreatedDate = seedDate
                    },
                    new Tag
                    {
                        TagId = 4,
                        Name = "Event",
                        Description = "Event related news",
                        CreatedDate = seedDate
                    }
                );
            });
        }
    }
} 