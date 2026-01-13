using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryAndStateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 1,
                column: "code",
                value: "USR_STS");

            migrationBuilder.UpdateData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 2,
                column: "code",
                value: "USR_RL");

            migrationBuilder.InsertData(
                table: "code_types",
                columns: new[] { "id", "code", "created_at", "created_by", "description", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 3, "", new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Country", null, null },
                    { 4, "STT", new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "State", null, null }
                });

            migrationBuilder.InsertData(
                table: "system_codes",
                columns: new[] { "id", "code", "code_type_id", "created_at", "created_by", "description", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 9, "MY", 3, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Malaysia", null, null },
                    { 10, "SG", 3, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Singapore", null, null },
                    { 11, "TH", 3, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thailand", null, null },
                    { 12, "ID", 3, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Indonesia", null, null },
                    { 13, "10", 4, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Selangor", null, null },
                    { 14, "11", 4, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Kuala Lumpur", null, null },
                    { 15, "12", 4, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Johor", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 1,
                column: "code",
                value: "USER_STATUS");

            migrationBuilder.UpdateData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 2,
                column: "code",
                value: "USER_ROLE");
        }
    }
}
