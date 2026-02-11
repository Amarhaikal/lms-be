using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class FixUserColumnOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE users 
                MODIFY COLUMN shortname VARCHAR(20) NULL 
                AFTER fullname;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE users 
                MODIFY COLUMN joined_dt DATE NULL 
                AFTER role_id;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE users 
                MODIFY COLUMN shortname VARCHAR(20) NULL 
                AFTER password_changed_at;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE users 
                MODIFY COLUMN joined_dt DATE NULL 
                AFTER shortname;
            ");
        }
    }
}
