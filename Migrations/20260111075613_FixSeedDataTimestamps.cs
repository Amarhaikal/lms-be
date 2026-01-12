using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedDataTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Reposition status_id column to appear after phone_no
            migrationBuilder.Sql(@"
                ALTER TABLE `users` 
                MODIFY COLUMN `status_id` int NOT NULL AFTER `phone_no`;
            ");

            migrationBuilder.UpdateData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 2,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 3,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 4,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 54, 43, 840, DateTimeKind.Utc).AddTicks(4160));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 54, 43, 840, DateTimeKind.Utc).AddTicks(6350));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 2,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 54, 43, 840, DateTimeKind.Utc).AddTicks(6440));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 3,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 54, 43, 840, DateTimeKind.Utc).AddTicks(6450));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 4,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 54, 43, 840, DateTimeKind.Utc).AddTicks(6450));
        }
    }
}
