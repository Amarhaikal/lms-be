using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "group_screens");

            migrationBuilder.DropTable(
                name: "user_groups");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_users_status",
                table: "users",
                column: "status");

            migrationBuilder.AddForeignKey(
                name: "FK_users_system_codes_status",
                table: "users",
                column: "status",
                principalTable: "system_codes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_system_codes_status",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_status",
                table: "users");

            migrationBuilder.DropColumn(
                name: "status",
                table: "users");

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "group_screens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    group_id = table.Column<int>(type: "int", nullable: false),
                    screen_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_screens", x => x.id);
                    table.ForeignKey(
                        name: "FK_group_screens_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_screens_screens_screen_id",
                        column: x => x.screen_id,
                        principalTable: "screens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    group_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_groups_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_groups_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "code_types",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 1, 5, 14, 16, 31, 458, DateTimeKind.Utc).AddTicks(1937));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 1, 5, 14, 16, 31, 458, DateTimeKind.Utc).AddTicks(6491));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 2,
                column: "created_at",
                value: new DateTime(2026, 1, 5, 14, 16, 31, 458, DateTimeKind.Utc).AddTicks(6641));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 3,
                column: "created_at",
                value: new DateTime(2026, 1, 5, 14, 16, 31, 458, DateTimeKind.Utc).AddTicks(6642));

            migrationBuilder.UpdateData(
                table: "system_codes",
                keyColumn: "id",
                keyValue: 4,
                column: "created_at",
                value: new DateTime(2026, 1, 5, 14, 16, 31, 458, DateTimeKind.Utc).AddTicks(6643));

            migrationBuilder.CreateIndex(
                name: "IX_group_screens_group_id_screen_id",
                table: "group_screens",
                columns: new[] { "group_id", "screen_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_screens_screen_id",
                table: "group_screens",
                column: "screen_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_groups_group_id",
                table: "user_groups",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_groups_user_id_group_id",
                table: "user_groups",
                columns: new[] { "user_id", "group_id" },
                unique: true);
        }
    }
}
