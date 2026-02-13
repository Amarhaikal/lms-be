using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class AddOfficerAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "users",
                type: "int",
                nullable: true)
                .Annotation("MySql:After", "gender_id");

            migrationBuilder.AddColumn<string>(
                name: "designation",
                table: "users",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:After", "department_id");

            migrationBuilder.AddColumn<string>(
                name: "remarks",
                table: "users",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:After", "designation");

            migrationBuilder.AddColumn<string>(
                name: "staff_id",
                table: "users",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:After", "username");

            migrationBuilder.CreateIndex(
                name: "IX_users_created_by",
                table: "users",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_users_department_id",
                table: "users",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_updated_by",
                table: "users",
                column: "updated_by");

            migrationBuilder.AddForeignKey(
                name: "FK_users_system_codes_department_id",
                table: "users",
                column: "department_id",
                principalTable: "system_codes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_users_users_created_by",
                table: "users",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_users_updated_by",
                table: "users",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_system_codes_department_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_users_created_by",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_users_updated_by",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_created_by",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_department_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_updated_by",
                table: "users");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "designation",
                table: "users");

            migrationBuilder.DropColumn(
                name: "remarks",
                table: "users");

            migrationBuilder.DropColumn(
                name: "staff_id",
                table: "users");
        }
    }
}
