using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserDeletionBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "FK_rates_users_created_by",
                table: "rates");

            migrationBuilder.DropForeignKey(
                name: "FK_rates_users_updated_by",
                table: "rates");

            migrationBuilder.DropForeignKey(
                name: "FK_system_codes_users_created_by",
                table: "system_codes");

            migrationBuilder.DropForeignKey(
                name: "FK_system_codes_users_updated_by",
                table: "system_codes");

            migrationBuilder.DropForeignKey(
                name: "FK_users_users_created_by",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_users_updated_by",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_user_id",
                table: "audit_logs",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_audit_logs_users_user_id",
                table: "audit_logs",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_code_types_users_created_by",
                table: "code_types",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_code_types_users_updated_by",
                table: "code_types",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_documents_users_created_by",
                table: "documents",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_documents_users_updated_by",
                table: "documents",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_rates_users_created_by",
                table: "rates",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_rates_users_updated_by",
                table: "rates",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_system_codes_users_created_by",
                table: "system_codes",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_system_codes_users_updated_by",
                table: "system_codes",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_users_users_created_by",
                table: "users",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_users_users_updated_by",
                table: "users",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_audit_logs_users_user_id",
                table: "audit_logs");

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
                name: "FK_rates_users_created_by",
                table: "rates");

            migrationBuilder.DropForeignKey(
                name: "FK_rates_users_updated_by",
                table: "rates");

            migrationBuilder.DropForeignKey(
                name: "FK_system_codes_users_created_by",
                table: "system_codes");

            migrationBuilder.DropForeignKey(
                name: "FK_system_codes_users_updated_by",
                table: "system_codes");

            migrationBuilder.DropForeignKey(
                name: "FK_users_users_created_by",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_users_updated_by",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_audit_logs_user_id",
                table: "audit_logs");

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
                name: "FK_rates_users_created_by",
                table: "rates",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_rates_users_updated_by",
                table: "rates",
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
    }
}
