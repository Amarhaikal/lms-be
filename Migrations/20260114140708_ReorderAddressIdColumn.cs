using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class ReorderAddressIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "id_no",
                table: "users",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(70)",
                oldMaxLength: 70)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            // Reorder address_id column to appear after phone_no
            migrationBuilder.Sql(@"
                ALTER TABLE users 
                MODIFY COLUMN address_id INT NULL 
                AFTER phone_no;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Move address_id back to the end (after updated_at)
            migrationBuilder.Sql(@"
                ALTER TABLE users 
                MODIFY COLUMN address_id INT NULL 
                AFTER updated_at;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "id_no",
                table: "users",
                type: "varchar(70)",
                maxLength: 70,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
