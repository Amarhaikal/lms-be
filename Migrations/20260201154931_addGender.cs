using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class addGender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int?>(
                name: "gender_id",
                table: "users",
                type: "int",
                nullable: true);

            // Force MySQL to move the column after phone_no
            migrationBuilder.Sql("ALTER TABLE users MODIFY COLUMN gender_id int NULL AFTER phone_no;");

            migrationBuilder.CreateIndex(
                name: "IX_users_gender_id",
                table: "users",
                column: "gender_id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_system_codes_gender_id",
                table: "users",
                column: "gender_id",
                principalTable: "system_codes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_system_codes_gender_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_gender_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "gender_id",
                table: "users");
        }
    }
}
