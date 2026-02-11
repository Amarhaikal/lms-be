using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class AddUserShortname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "shortname",
                table: "users",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:After", "fullname");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "shortname",
                table: "users");
        }
    }
}
