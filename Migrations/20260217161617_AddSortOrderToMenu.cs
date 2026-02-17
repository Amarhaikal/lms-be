using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class AddSortOrderToMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                table: "menus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "menus",
                keyColumn: "id",
                keyValue: 1,
                column: "sort_order",
                value: 0);

            migrationBuilder.UpdateData(
                table: "menus",
                keyColumn: "id",
                keyValue: 2,
                column: "sort_order",
                value: 0);

            migrationBuilder.UpdateData(
                table: "menus",
                keyColumn: "id",
                keyValue: 3,
                column: "sort_order",
                value: 0);

            migrationBuilder.UpdateData(
                table: "menus",
                keyColumn: "id",
                keyValue: 4,
                column: "sort_order",
                value: 0);

            migrationBuilder.UpdateData(
                table: "menus",
                keyColumn: "id",
                keyValue: 5,
                column: "sort_order",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sort_order",
                table: "menus");
        }
    }
}
