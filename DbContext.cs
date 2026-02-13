using Microsoft.EntityFrameworkCore;
using QUANTM.Models.User;
using QUANTM.Models.Parameter;
using QUANTM.Models.Common;

namespace QUANTM.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // User Access Control
        public DbSet<User> Users { get; set; }

        // Common
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuRole> MenuRoles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Document> Documents { get; set; }

        // Parameters
        public DbSet<CodeType> CodeTypes { get; set; }
        public DbSet<SystemCode> SystemCodes { get; set; }

        // Sessions
        public DbSet<Models.Session.Session> Sessions { get; set; }

        // Audit Logs
        public DbSet<Models.Audit.AuditLog> AuditLogs { get; set; }

        // Auth
        public DbSet<Models.Auth.PasswordHistory> PasswordHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships for SystemCode
            modelBuilder.Entity<SystemCode>()
                .HasOne(sc => sc.CodeType)
                .WithMany(ct => ct.SystemCodes)
                .HasForeignKey(sc => sc.CodeTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany()
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Audit relationships for SystemCode
            modelBuilder.Entity<SystemCode>(entity =>
            {
                entity.HasOne(sc => sc.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(sc => sc.CreatedBy);

                entity.HasOne(sc => sc.UpdatedByUser)
                    .WithMany()
                    .HasForeignKey(sc => sc.UpdatedBy);
            });

            // Configure Audit relationships for CodeType
            modelBuilder.Entity<CodeType>(entity =>
            {
                entity.HasOne(ct => ct.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(ct => ct.CreatedBy);

                entity.HasOne(ct => ct.UpdatedByUser)
                    .WithMany()
                    .HasForeignKey(ct => ct.UpdatedBy);
            });

            // Configure Audit relationships for Document
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedBy);

                entity.HasOne(d => d.UpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.UpdatedBy);
            });

            // Configure relationships for MenuRole
            modelBuilder.Entity<MenuRole>()
                .HasOne(mr => mr.Menu)
                .WithMany(m => m.MenuRoles)
                .HasForeignKey(mr => mr.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MenuRole>()
                .HasOne(mr => mr.Role)
                .WithMany(r => r.MenuRoles)
                .HasForeignKey(mr => mr.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships for Menu
            modelBuilder.Entity<Menu>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();

                entity.HasOne(m => m.Parent)
                      .WithMany(m => m.SubMenus)
                      .HasForeignKey(m => m.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed data for CodeTypes
            modelBuilder.Entity<CodeType>().HasData(
                new CodeType
                {
                    Id = 1,
                    Code = "USR_STS",
                    Description = "User Status Types",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new CodeType
                {
                    Id = 2,
                    Code = "USR_RL",
                    Description = "User Role Types",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new CodeType
                {
                    Id = 3,
                    Code = "CTRY",
                    Description = "Country",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new CodeType
                {
                    Id = 4,
                    Code = "STT",
                    Description = "State",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new CodeType
                {
                    Id = 5,
                    Code = "GNDR",
                    Description = "Gender",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed data for SystemCodes
            modelBuilder.Entity<SystemCode>().HasData(
                // User Status
                new SystemCode
                {
                    Id = 1,
                    CodeTypeId = 1,
                    Code = "ACTIVE",
                    Description = "Active",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 2,
                    CodeTypeId = 1,
                    Code = "INACTIVE",
                    Description = "Inactive",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 3,
                    CodeTypeId = 1,
                    Code = "SUSPENDED",
                    Description = "Suspended",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 4,
                    CodeTypeId = 1,
                    Code = "NEW",
                    Description = "New",
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
                    Id = 8,
                    CodeTypeId = 2,
                    Code = "ADM",
                    Description = "Admin",
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

                // Country
                new SystemCode
                {
                    Id = 9,
                    CodeTypeId = 3,
                    Code = "MY",
                    Description = "Malaysia",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 10,
                    CodeTypeId = 3,
                    Code = "SG",
                    Description = "Singapore",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 11,
                    CodeTypeId = 3,
                    Code = "TH",
                    Description = "Thailand",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 12,
                    CodeTypeId = 3,
                    Code = "ID",
                    Description = "Indonesia",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                // State
                new SystemCode
                {
                    Id = 13,
                    CodeTypeId = 4,
                    Code = "10",
                    Description = "Selangor",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 14,
                    CodeTypeId = 4,
                    Code = "11",
                    Description = "Kuala Lumpur",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 15,
                    CodeTypeId = 4,
                    Code = "12",
                    Description = "Johor",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                // Gender
                new SystemCode
                {
                    Id = 16,
                    CodeTypeId = 5,
                    Code = "M",
                    Description = "Male",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new SystemCode
                {
                    Id = 17,
                    CodeTypeId = 5,
                    Code = "F",
                    Description = "Female",
                    CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed data for Menus
            modelBuilder.Entity<Menu>().HasData(
                new Menu { Id = 1, Name = "Settings", Code = "SETTINGS", Icon = "settings", CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new Menu { Id = 2, ParentId = 1, Name = "Profile", Code = "PROFILE", Url = "/settings/profile", Icon = "person", CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new Menu { Id = 3, Name = "Administrator", Code = "ADMIN", Icon = "admin_panel_settings", CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new Menu { Id = 4, ParentId = 3, Name = "Users", Code = "USERS", Url = "/admin/users", Icon = "group", CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new Menu { Id = 5, ParentId = 3, Name = "System Codes", Code = "SYSTEM_CODES", Url = "/admin/system-codes", Icon = "terminal", CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) }
            );

            // Seed data for MenuRoles
            modelBuilder.Entity<MenuRole>().HasData(
                // Settings & Profile - All Roles (SA: 5, ADM: 8, OFCR: 6, SPRVSR: 7)
                new MenuRole { Id = 1, MenuId = 1, RoleId = 5, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new MenuRole { Id = 2, MenuId = 1, RoleId = 8, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new MenuRole { Id = 3, MenuId = 1, RoleId = 6, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new MenuRole { Id = 4, MenuId = 1, RoleId = 7, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },

                new MenuRole { Id = 5, MenuId = 2, RoleId = 5, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new MenuRole { Id = 6, MenuId = 2, RoleId = 8, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new MenuRole { Id = 7, MenuId = 2, RoleId = 6, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new MenuRole { Id = 8, MenuId = 2, RoleId = 7, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },

                // Administrator - Super Admin & Admin (5, 8)
                new MenuRole { Id = 9, MenuId = 3, RoleId = 5, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new MenuRole { Id = 10, MenuId = 3, RoleId = 8, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },

                // Users - Only Admin & Super Admin (5, 8)
                new MenuRole { Id = 11, MenuId = 4, RoleId = 5, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new MenuRole { Id = 12, MenuId = 4, RoleId = 8, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },

                // System Codes - All Roles (5, 8, 6, 7)
                new MenuRole { Id = 13, MenuId = 5, RoleId = 5, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new MenuRole { Id = 14, MenuId = 5, RoleId = 8, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new MenuRole { Id = 15, MenuId = 5, RoleId = 6, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new MenuRole { Id = 16, MenuId = 5, RoleId = 7, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },

                // Ensure Administrator parent is also visible for others who see System Codes
                new MenuRole { Id = 17, MenuId = 3, RoleId = 6, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new MenuRole { Id = 18, MenuId = 3, RoleId = 7, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) }
            );

        }
    }
}
