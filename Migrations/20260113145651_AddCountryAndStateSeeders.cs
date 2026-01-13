using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryAndStateSeeders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 3,
                column: "code",
                value: "CTRY");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 3,
                column: "code",
                value: "");
        }
    }
}
