using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KITT.Core.Migrations
{
    public partial class CreateStreamingEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Streamings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TwitchChannel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    HostingChannelUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    YouTubeVideoUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Abstract = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streamings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Streamings_Slug",
                table: "Streamings",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Streamings_Title",
                table: "Streamings",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Streamings_TwitchChannel",
                table: "Streamings",
                column: "TwitchChannel");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Streamings");
        }
    }
}
