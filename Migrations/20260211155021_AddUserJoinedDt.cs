using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANTM.Migrations
{
    /// <inheritdoc />
    public partial class AddUserJoinedDt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "joined_dt",
                table: "users",
                type: "date",
                nullable: true)
                .Annotation("MySql:After", "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "joined_dt",
                table: "users");
        }
    }
}
