using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserStatusCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 1,
                column: "code",
                value: "A");

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 2,
                column: "code",
                value: "I");

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 3,
                column: "code",
                value: "S");

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 4,
                column: "code",
                value: "N");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 1,
                column: "code",
                value: "ACTIVE");

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 2,
                column: "code",
                value: "INACTIVE");

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 3,
                column: "code",
                value: "SUSPENDED");

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 4,
                column: "code",
                value: "NEW");
        }
    }
}
