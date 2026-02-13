using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class CleanOrphanedAuditData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clean up orphaned audit references to avoid FK constraint violations
            // This handles cases where ID = 0 or any other non-existent user ID
            migrationBuilder.Sql("UPDATE code_types SET created_by = NULL WHERE created_by IS NOT NULL AND created_by NOT IN (SELECT id FROM users);");
            migrationBuilder.Sql("UPDATE code_types SET updated_by = NULL WHERE updated_by IS NOT NULL AND updated_by NOT IN (SELECT id FROM users);");

            migrationBuilder.Sql("UPDATE system_codes SET created_by = NULL WHERE created_by IS NOT NULL AND created_by NOT IN (SELECT id FROM users);");
            migrationBuilder.Sql("UPDATE system_codes SET updated_by = NULL WHERE updated_by IS NOT NULL AND updated_by NOT IN (SELECT id FROM users);");

            migrationBuilder.Sql("UPDATE documents SET created_by = NULL WHERE created_by IS NOT NULL AND created_by NOT IN (SELECT id FROM users);");
            migrationBuilder.Sql("UPDATE documents SET updated_by = NULL WHERE updated_by IS NOT NULL AND updated_by NOT IN (SELECT id FROM users);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
