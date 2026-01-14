using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class RenameAddressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_system_codes_country_id",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_system_codes_state_id",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_users_Addresses_address_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "Addresses",
                newName: "addresses");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_state_id",
                table: "addresses",
                newName: "IX_addresses_state_id");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_country_id",
                table: "addresses",
                newName: "IX_addresses_country_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_addresses",
                table: "addresses",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_addresses_system_codes_country_id",
                table: "addresses",
                column: "country_id",
                principalTable: "system_codes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_addresses_system_codes_state_id",
                table: "addresses",
                column: "state_id",
                principalTable: "system_codes",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_addresses_address_id",
                table: "users",
                column: "address_id",
                principalTable: "addresses",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_addresses_system_codes_country_id",
                table: "addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_addresses_system_codes_state_id",
                table: "addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_users_addresses_address_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_addresses",
                table: "addresses");

            migrationBuilder.RenameTable(
                name: "addresses",
                newName: "Addresses");

            migrationBuilder.RenameIndex(
                name: "IX_addresses_state_id",
                table: "Addresses",
                newName: "IX_Addresses_state_id");

            migrationBuilder.RenameIndex(
                name: "IX_addresses_country_id",
                table: "Addresses",
                newName: "IX_Addresses_country_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_system_codes_country_id",
                table: "Addresses",
                column: "country_id",
                principalTable: "system_codes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_system_codes_state_id",
                table: "Addresses",
                column: "state_id",
                principalTable: "system_codes",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_Addresses_address_id",
                table: "users",
                column: "address_id",
                principalTable: "Addresses",
                principalColumn: "id");
        }
    }
}
