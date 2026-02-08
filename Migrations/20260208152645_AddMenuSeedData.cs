using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "screens");

            migrationBuilder.CreateTable(
                name: "menus",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    icon = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    parent_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menus", x => x.id);
                    table.ForeignKey(
                        name: "FK_menus_menus_parent_id",
                        column: x => x.parent_id,
                        principalTable: "menus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "menu_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    menu_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_menu_roles_menus_menu_id",
                        column: x => x.menu_id,
                        principalTable: "menus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_menu_roles_system_codes_role_id",
                        column: x => x.role_id,
                        principalTable: "system_codes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "menus",
                columns: new[] { "id", "code", "created_at", "created_by", "icon", "name", "parent_id", "updated_at", "updated_by", "url" },
                values: new object[,]
                {
                    { 1, "SETTINGS", new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "settings", "Settings", null, null, null, null },
                    { 3, "ADMIN", new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin_panel_settings", "Administrator", null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "menu_roles",
                columns: new[] { "id", "created_at", "created_by", "menu_id", "role_id", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, 5, null, null },
                    { 2, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, 8, null, null },
                    { 3, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, 6, null, null },
                    { 4, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, 7, null, null },
                    { 9, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, 5, null, null },
                    { 10, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, 8, null, null },
                    { 17, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, 6, null, null },
                    { 18, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, 7, null, null }
                });

            migrationBuilder.InsertData(
                table: "menus",
                columns: new[] { "id", "code", "created_at", "created_by", "icon", "name", "parent_id", "updated_at", "updated_by", "url" },
                values: new object[,]
                {
                    { 2, "PROFILE", new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "person", "Profile", 1, null, null, "/settings/profile" },
                    { 4, "USERS", new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "group", "Users", 3, null, null, "/admin/users" },
                    { 5, "SYSTEM_CODES", new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "terminal", "System Codes", 3, null, null, "/admin/system-codes" }
                });

            migrationBuilder.InsertData(
                table: "menu_roles",
                columns: new[] { "id", "created_at", "created_by", "menu_id", "role_id", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 5, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, 5, null, null },
                    { 6, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, 8, null, null },
                    { 7, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, 6, null, null },
                    { 8, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, 7, null, null },
                    { 11, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, 5, null, null },
                    { 12, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, 8, null, null },
                    { 13, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, 5, null, null },
                    { 14, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, 8, null, null },
                    { 15, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, 6, null, null },
                    { 16, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, 7, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_menu_roles_menu_id",
                table: "menu_roles",
                column: "menu_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_roles_role_id",
                table: "menu_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_menus_code",
                table: "menus",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_menus_parent_id",
                table: "menus",
                column: "parent_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "menu_roles");

            migrationBuilder.DropTable(
                name: "menus");

            migrationBuilder.CreateTable(
                name: "screens",
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
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_screens", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
