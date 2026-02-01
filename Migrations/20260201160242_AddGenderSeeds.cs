using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "code_types",
                columns: new[] { "id", "code", "created_at", "created_by", "description", "updated_at", "updated_by" },
                values: new object[] { 5, "GNDR", new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Gender", null, null });

            migrationBuilder.InsertData(
                table: "system_codes",
                columns: new[] { "id", "code", "code_type_id", "created_at", "created_by", "description", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 16, "M", 5, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Male", null, null },
                    { 17, "F", 5, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Female", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 5);
        }
    }
}
