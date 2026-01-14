using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSystemCodeSeederData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 1,
                column: "description",
                value: "Active");

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 2,
                column: "description",
                value: "Inactive");

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 3,
                column: "description",
                value: "Suspended");

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "code", "description" },
                values: new object[] { "NEW", "New" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 1,
                column: "description",
                value: "Active User");

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 2,
                column: "description",
                value: "Inactive User");

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 3,
                column: "description",
                value: "Suspended User");

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "code", "description" },
                values: new object[] { "PENDING", "Pending Approval" });

            migrationBuilder.InsertData(
                table: "system_codes",
                columns: new[] { "id", "code", "code_type_id", "created_at", "created_by", "description", "updated_at", "updated_by" },
                values: new object[] { 8, "PADM", 2, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Parameter Admin", null, null });
        }
    }
}
