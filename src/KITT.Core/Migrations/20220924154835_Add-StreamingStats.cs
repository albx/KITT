using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KITT.Core.Migrations
{
    public partial class AddStreamingStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KITT_StreamingStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StreamingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Viewers = table.Column<int>(type: "int", nullable: false),
                    Subscribers = table.Column<int>(type: "int", nullable: false),
                    UserJoinedNumber = table.Column<int>(type: "int", nullable: false),
                    UserLeftNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KITT_StreamingStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KITT_StreamingStats_KITT_Streamings_StreamingId",
                        column: x => x.StreamingId,
                        principalTable: "KITT_Streamings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_KITT_StreamingStats_StreamingId",
                table: "KITT_StreamingStats",
                column: "StreamingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KITT_StreamingStats");
        }
    }
}
