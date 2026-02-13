using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class ReorderOfficerColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE users 
                MODIFY COLUMN staff_id VARCHAR(50) NULL AFTER username;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE users 
                MODIFY COLUMN department_id INT NULL AFTER gender_id;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE users 
                MODIFY COLUMN designation VARCHAR(100) NULL AFTER department_id;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE users 
                MODIFY COLUMN remarks VARCHAR(255) NULL AFTER designation;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
