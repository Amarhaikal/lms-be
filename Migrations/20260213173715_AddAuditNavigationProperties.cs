using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clean up orphaned audit references to avoid FK constraint violations
            migrationBuilder.Sql("UPDATE code_types SET created_by = NULL WHERE created_by IS NOT NULL AND created_by NOT IN (SELECT id FROM users);");
            migrationBuilder.Sql("UPDATE code_types SET updated_by = NULL WHERE updated_by IS NOT NULL AND updated_by NOT IN (SELECT id FROM users);");
            migrationBuilder.Sql("UPDATE system_codes SET created_by = NULL WHERE created_by IS NOT NULL AND created_by NOT IN (SELECT id FROM users);");
            migrationBuilder.Sql("UPDATE system_codes SET updated_by = NULL WHERE updated_by IS NOT NULL AND updated_by NOT IN (SELECT id FROM users);");
            migrationBuilder.Sql("UPDATE documents SET created_by = NULL WHERE created_by IS NOT NULL AND created_by NOT IN (SELECT id FROM users);");
            migrationBuilder.Sql("UPDATE documents SET updated_by = NULL WHERE updated_by IS NOT NULL AND updated_by NOT IN (SELECT id FROM users);");

            /*
            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "documents",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "updated_by",
                table: "documents",
                type: "int",
                nullable: true);
            */

            /*
            migrationBuilder.CreateIndex(
                name: "IX_system_codes_created_by",
                table: "system_codes",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_system_codes_updated_by",
                table: "system_codes",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_documents_created_by",
                table: "documents",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_documents_updated_by",
                table: "documents",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_code_types_created_by",
                table: "code_types",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_code_types_updated_by",
                table: "code_types",
                column: "updated_by");
            */

            migrationBuilder.AddForeignKey(
                name: "FK_code_types_users_created_by",
                table: "code_types",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_code_types_users_updated_by",
                table: "code_types",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_users_created_by",
                table: "documents",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_users_updated_by",
                table: "documents",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_system_codes_users_created_by",
                table: "system_codes",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_system_codes_users_updated_by",
                table: "system_codes",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_code_types_users_created_by",
                table: "code_types");

            migrationBuilder.DropForeignKey(
                name: "FK_code_types_users_updated_by",
                table: "code_types");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_users_created_by",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_users_updated_by",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "FK_system_codes_users_created_by",
                table: "system_codes");

            migrationBuilder.DropForeignKey(
                name: "FK_system_codes_users_updated_by",
                table: "system_codes");

            /*
            migrationBuilder.DropIndex(
                name: "IX_system_codes_created_by",
                table: "system_codes");

            migrationBuilder.DropIndex(
                name: "IX_system_codes_updated_by",
                table: "system_codes");

            migrationBuilder.DropIndex(
                name: "IX_documents_created_by",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "IX_documents_updated_by",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "IX_code_types_created_by",
                table: "code_types");

            migrationBuilder.DropIndex(
                name: "IX_code_types_updated_by",
                table: "code_types");
            */

            /*
            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "documents");
            */
        }
    }
}
