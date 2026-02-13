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
            // 1. Ensure columns and indices exist (Safely handle production catch-up)
            // This uses PREPARE/EXECUTE because CREATE PROCEDURE is not allowed inside EF Core's idempotent script.

            // Add missing columns to documents
            migrationBuilder.Sql(@"
                SET @s = (SELECT IF(
                    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'documents' AND COLUMN_NAME = 'updated_by') = 0,
                    'ALTER TABLE documents ADD updated_by int NULL',
                    'DO 0'
                ));
                PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");
            migrationBuilder.Sql(@"
                SET @s = (SELECT IF(
                    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'documents' AND COLUMN_NAME = 'updated_at') = 0,
                    'ALTER TABLE documents ADD updated_at datetime(6) NULL',
                    'DO 0'
                ));
                PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");

            // Add missing indices to system_codes
            migrationBuilder.Sql(@"
                SET @s = (SELECT IF(
                    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'system_codes' AND INDEX_NAME = 'IX_system_codes_created_by') = 0,
                    'CREATE INDEX IX_system_codes_created_by ON system_codes (created_by)',
                    'DO 0'
                ));
                PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");
            migrationBuilder.Sql(@"
                SET @s = (SELECT IF(
                    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'system_codes' AND INDEX_NAME = 'IX_system_codes_updated_by') = 0,
                    'CREATE INDEX IX_system_codes_updated_by ON system_codes (updated_by)',
                    'DO 0'
                ));
                PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");

            // Add missing indices to documents
            migrationBuilder.Sql(@"
                SET @s = (SELECT IF(
                    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'documents' AND INDEX_NAME = 'IX_documents_created_by') = 0,
                    'CREATE INDEX IX_documents_created_by ON documents (created_by)',
                    'DO 0'
                ));
                PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");
            migrationBuilder.Sql(@"
                SET @s = (SELECT IF(
                    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'documents' AND INDEX_NAME = 'IX_documents_updated_by') = 0,
                    'CREATE INDEX IX_documents_updated_by ON documents (updated_by)',
                    'DO 0'
                ));
                PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");

            // Add missing indices to code_types
            migrationBuilder.Sql(@"
                SET @s = (SELECT IF(
                    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'code_types' AND INDEX_NAME = 'IX_code_types_created_by') = 0,
                    'CREATE INDEX IX_code_types_created_by ON code_types (created_by)',
                    'DO 0'
                ));
                PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");
            migrationBuilder.Sql(@"
                SET @s = (SELECT IF(
                    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'code_types' AND INDEX_NAME = 'IX_code_types_updated_by') = 0,
                    'CREATE INDEX IX_code_types_updated_by ON code_types (updated_by)',
                    'DO 0'
                ));
                PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");

            // 2. Clean up orphaned audit references to avoid FK constraint violations
            migrationBuilder.Sql("UPDATE code_types SET created_by = NULL WHERE created_by IS NOT NULL AND created_by NOT IN (SELECT id FROM users);");
            migrationBuilder.Sql("UPDATE code_types SET updated_by = NULL WHERE updated_by IS NOT NULL AND updated_by NOT IN (SELECT id FROM users);");
            migrationBuilder.Sql("UPDATE system_codes SET created_by = NULL WHERE created_by IS NOT NULL AND created_by NOT IN (SELECT id FROM users);");
            migrationBuilder.Sql("UPDATE system_codes SET updated_by = NULL WHERE updated_by IS NOT NULL AND updated_by NOT IN (SELECT id FROM users);");
            migrationBuilder.Sql("UPDATE documents SET created_by = NULL WHERE created_by IS NOT NULL AND created_by NOT IN (SELECT id FROM users);");
            migrationBuilder.Sql("UPDATE documents SET updated_by = NULL WHERE updated_by IS NOT NULL AND updated_by NOT IN (SELECT id FROM users);");

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
