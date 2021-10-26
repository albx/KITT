using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KITT.Core.Migrations
{
    public partial class AddSettingsModelAndUserIdInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Streamings",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    TwitchChannel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Streamings_UserId",
                table: "Streamings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_TwitchChannel",
                table: "Settings",
                column: "TwitchChannel");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_UserId",
                table: "Settings",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropIndex(
                name: "IX_Streamings_UserId",
                table: "Streamings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Streamings");
        }
    }
}
