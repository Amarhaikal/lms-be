using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class RenameStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_system_codes_status",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "users",
                newName: "status_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_status",
                table: "users",
                newName: "IX_users_status_id");

            migrationBuilder.UpdateData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 54, 43, 840, DateTimeKind.Utc).AddTicks(4160));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 54, 43, 840, DateTimeKind.Utc).AddTicks(6350));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 2,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 54, 43, 840, DateTimeKind.Utc).AddTicks(6440));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 3,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 54, 43, 840, DateTimeKind.Utc).AddTicks(6450));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 4,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 54, 43, 840, DateTimeKind.Utc).AddTicks(6450));

            migrationBuilder.AddForeignKey(
                name: "FK_users_system_codes_status_id",
                table: "users",
                column: "status_id",
                principalTable: "system_codes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_system_codes_status_id",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "status_id",
                table: "users",
                newName: "status");

            migrationBuilder.RenameIndex(
                name: "IX_users_status_id",
                table: "users",
                newName: "IX_users_status");

            migrationBuilder.UpdateData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 51, 2, 817, DateTimeKind.Utc).AddTicks(5850));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 51, 2, 817, DateTimeKind.Utc).AddTicks(8080));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 2,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 51, 2, 817, DateTimeKind.Utc).AddTicks(8170));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 3,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 51, 2, 817, DateTimeKind.Utc).AddTicks(8170));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 4,
                column: "created_at",
                value: new DateTime(2026, 1, 11, 7, 51, 2, 817, DateTimeKind.Utc).AddTicks(8170));

            migrationBuilder.AddForeignKey(
                name: "FK_users_system_codes_status",
                table: "users",
                column: "status",
                principalTable: "system_codes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
