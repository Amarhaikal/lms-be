using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_system_codes_gender_id",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "gender_id",
                table: "users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_users_system_codes_gender_id",
                table: "users",
                column: "gender_id",
                principalTable: "system_codes",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_system_codes_gender_id",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "gender_id",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_system_codes_gender_id",
                table: "users",
                column: "gender_id",
                principalTable: "system_codes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
