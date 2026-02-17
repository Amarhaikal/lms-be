using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class ReorderMenuSortOrderColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE menus 
                MODIFY COLUMN sort_order INT NOT NULL DEFAULT 0 
                AFTER parent_id;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE menus 
                MODIFY COLUMN sort_order INT NOT NULL DEFAULT 0 
                AFTER updated_at;
            ");
        }
    }
}
