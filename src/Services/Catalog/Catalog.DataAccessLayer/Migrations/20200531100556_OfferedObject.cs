using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.DataAccessLayer.Migrations
{
    public partial class OfferedObject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObjectFreeProperties",
                columns: table => new
                {
                    ObjectId = table.Column<int>(nullable: false),
                    TakerId = table.Column<Guid>(nullable: true),
                    TakenAtUtc = table.Column<DateTime>(nullable: true),
                    OfferedFreeAtUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectFreeProperties", x => x.ObjectId);
                    table.ForeignKey(
                        name: "FK_ObjectFreeProperties_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "OfferedObjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectFreeProperties_Users_TakerId",
                        column: x => x.TakerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectFreeProperties_TakerId",
                table: "ObjectFreeProperties",
                column: "TakerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectFreeProperties");
        }
    }
}
