using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KITT.Core.Migrations
{
    /// <inheritdoc />
    public partial class RefactorStreamings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KITT_Expenses");

            migrationBuilder.DropTable(
                name: "KITT_StreamingStats");

            migrationBuilder.RenameColumn(
                name: "YouTubeVideoUrl",
                table: "KITT_Streamings",
                newName: "YouTubeUrl");

            migrationBuilder.RenameColumn(
                name: "HostingChannelUrl",
                table: "KITT_Streamings",
                newName: "TwitchUrl");

            migrationBuilder.AddColumn<string>(
                name: "YouTubeChannel",
                table: "KITT_Streamings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorNickname",
                table: "KITT_Proposals",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YouTubeChannel",
                table: "KITT_Streamings");

            migrationBuilder.RenameColumn(
                name: "YouTubeUrl",
                table: "KITT_Streamings",
                newName: "YouTubeVideoUrl");

            migrationBuilder.RenameColumn(
                name: "TwitchUrl",
                table: "KITT_Streamings",
                newName: "HostingChannelUrl");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorNickname",
                table: "KITT_Proposals",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "KITT_Expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ExpenseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentInfo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KITT_Expenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KITT_StreamingStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StreamingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Subscribers = table.Column<int>(type: "int", nullable: false),
                    UserJoinedNumber = table.Column<int>(type: "int", nullable: false),
                    UserLeftNumber = table.Column<int>(type: "int", nullable: false),
                    Viewers = table.Column<int>(type: "int", nullable: false)
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
    }
}
