using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert USER_ROLE code type if it doesn't exist
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO `code_types` (`id`, `code`, `created_at`, `created_by`, `description`, `updated_at`, `updated_by`)
                VALUES (2, 'USER_ROLE', '2026-01-11 00:00:00', NULL, 'User Role Types', NULL, NULL);
            ");

            // Insert user role system codes if they don't exist
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO `system_codes` (`id`, `code`, `code_type_id`, `created_at`, `created_by`, `description`, `updated_at`, `updated_by`)
                VALUES 
                    (5, 'SA', 2, '2026-01-11 00:00:00', NULL, 'Super Admin', NULL, NULL),
                    (6, 'OFCR', 2, '2026-01-11 00:00:00', NULL, 'Officer', NULL, NULL),
                    (7, 'SPRVSR', 2, '2026-01-11 00:00:00', NULL, 'Supervisor', NULL, NULL),
                    (8, 'PADM', 2, '2026-01-11 00:00:00', NULL, 'Parameter Admin', NULL, NULL);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 2);
        }
    }
}
