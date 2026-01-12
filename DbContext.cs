using Microsoft.EntityFrameworkCore;
using LMS.Models.User;
using LMS.Models.Parameter;

namespace LMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // User Access Control
        public DbSet<User> Users { get; set; }
        public DbSet<Screen> Screens { get; set; }

        // Parameters
        public DbSet<CodeType> CodeTypes { get; set; }
        public DbSet<SystemCode> SystemCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships for SystemCode
            modelBuilder.Entity<SystemCode>()
                .HasOne(sc => sc.CodeType)
                .WithMany(ct => ct.SystemCodes)
                .HasForeignKey(sc => sc.CodeTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data for CodeTypes
            modelBuilder.Entity<CodeType>().HasData(
                new CodeType
                {
                    Id = 1,
                    Code = "USER_STATUS",
                    Description = "User Status Types",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new CodeType
                {
                    Id = 2,
                    Code = "USER_ROLE",
                    Description = "User Role Types",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed data for SystemCodes - User Status
            modelBuilder.Entity<SystemCode>().HasData(
                new SystemCode
                {
                    Id = 1,
                    CodeTypeId = 1,
                    Code = "ACTIVE",
                    Description = "Active User",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 2,
                    CodeTypeId = 1,
                    Code = "INACTIVE",
                    Description = "Inactive User",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 3,
                    CodeTypeId = 1,
                    Code = "SUSPENDED",
                    Description = "Suspended User",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 4,
                    CodeTypeId = 1,
                    Code = "PENDING",
                    Description = "Pending Approval",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                // User Roles
                new SystemCode
                {
                    Id = 5,
                    CodeTypeId = 2,
                    Code = "SA",
                    Description = "Super Admin",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 6,
                    CodeTypeId = 2,
                    Code = "OFCR",
                    Description = "Officer",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 7,
                    CodeTypeId = 2,
                    Code = "SPRVSR",
                    Description = "Supervisor",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 8,
                    CodeTypeId = 2,
                    Code = "PADM",
                    Description = "Parameter Admin",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
