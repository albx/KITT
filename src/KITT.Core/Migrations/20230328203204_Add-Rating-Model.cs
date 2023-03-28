using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KITT.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KITT_Streamings_KITT_Contents_Id",
                table: "KITT_Streamings");

            migrationBuilder.CreateTable(
                name: "KITT_Ratings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PageUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumberOfLikes = table.Column<int>(type: "int", nullable: false),
                    NumberOfDislikes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KITT_Ratings", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_KITT_Streamings_KITT_Contents_Id",
                table: "KITT_Streamings",
                column: "Id",
                principalTable: "KITT_Contents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KITT_Streamings_KITT_Contents_Id",
                table: "KITT_Streamings");

            migrationBuilder.DropTable(
                name: "KITT_Ratings");

            migrationBuilder.AddForeignKey(
                name: "FK_KITT_Streamings_KITT_Contents_Id",
                table: "KITT_Streamings",
                column: "Id",
                principalTable: "KITT_Contents",
                principalColumn: "Id");
        }
    }
}
