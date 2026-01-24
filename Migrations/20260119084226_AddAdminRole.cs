using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "system_codes",
                columns: new[] { "id", "code", "code_type_id", "created_at", "created_by", "description", "updated_at", "updated_by" },
                values: new object[] { 8, "ADM", 2, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Admin", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 8);
        }
    }
}
