using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class MakeCountryIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_addresses_system_codes_country_id",
                table: "addresses");

            migrationBuilder.AlterColumn<int>(
                name: "country_id",
                table: "addresses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_addresses_system_codes_country_id",
                table: "addresses",
                column: "country_id",
                principalTable: "system_codes",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_addresses_system_codes_country_id",
                table: "addresses");

            migrationBuilder.AlterColumn<int>(
                name: "country_id",
                table: "addresses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_addresses_system_codes_country_id",
                table: "addresses",
                column: "country_id",
                principalTable: "system_codes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
