using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddConfirmationTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Confirmations",
                columns: table => new
                {
                    ConfirmationTokenId = table.Column<Guid>(nullable: false),
                    ConfirmationCode = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false),
                    ConfirmationType = table.Column<string>(nullable: true),
                    IssuedAtUtc = table.Column<DateTime>(nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Confirmations", x => x.ConfirmationTokenId);
                    table.ForeignKey(
                        name: "FK_Confirmations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Confirmations_ConfirmationCode",
                table: "Confirmations",
                column: "ConfirmationCode");

            migrationBuilder.CreateIndex(
                name: "IX_Confirmations_UserId",
                table: "Confirmations",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Confirmations");
        }
    }
}
