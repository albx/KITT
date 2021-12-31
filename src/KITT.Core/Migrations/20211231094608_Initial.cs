using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KITT.Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KITT_Contents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Abstract = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seo_Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seo_Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seo_Keywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KITT_Contents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KITT_Settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    TwitchChannel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KITT_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KITT_Streamings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TwitchChannel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    HostingChannelUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    YouTubeVideoUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KITT_Streamings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KITT_Streamings_KITT_Contents_Id",
                        column: x => x.Id,
                        principalTable: "KITT_Contents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_KITT_Contents_Slug",
                table: "KITT_Contents",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KITT_Contents_Title",
                table: "KITT_Contents",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_KITT_Contents_UserId",
                table: "KITT_Contents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KITT_Settings_TwitchChannel",
                table: "KITT_Settings",
                column: "TwitchChannel");

            migrationBuilder.CreateIndex(
                name: "IX_KITT_Settings_UserId",
                table: "KITT_Settings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KITT_Streamings_TwitchChannel",
                table: "KITT_Streamings",
                column: "TwitchChannel");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KITT_Settings");

            migrationBuilder.DropTable(
                name: "KITT_Streamings");

            migrationBuilder.DropTable(
                name: "KITT_Contents");
        }
    }
}
