using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.DataAccessLayer.Migrations
{
    public partial class AddImpressions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objects_Users_OwnerId",
                table: "Objects");

            migrationBuilder.CreateTable(
                name: "ObjectImpressions",
                columns: table => new
                {
                    ObjectId = table.Column<int>(nullable: false),
                    LoginId = table.Column<Guid>(nullable: false),
                    ViewedAtUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectImpressions", x => new { x.ObjectId, x.LoginId, x.ViewedAtUtc });
                    table.ForeignKey(
                        name: "FK_ObjectImpressions_Logins_LoginId",
                        column: x => x.LoginId,
                        principalTable: "Logins",
                        principalColumn: "LoginId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectImpressions_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "OfferedObjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectImpressions_LoginId",
                table: "ObjectImpressions",
                column: "LoginId");

            migrationBuilder.AddForeignKey(
                name: "FK_Objects_Users_OwnerId",
                table: "Objects",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objects_Users_OwnerId",
                table: "Objects");

            migrationBuilder.DropTable(
                name: "ObjectImpressions");

            migrationBuilder.AddForeignKey(
                name: "FK_Objects_Users_OwnerId",
                table: "Objects",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
