using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.DataAccessLayer.Migrations
{
    public partial class AddLikes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObjectLike",
                columns: table => new
                {
                    ObjectLikeId = table.Column<Guid>(nullable: false),
                    ObjectId = table.Column<int>(nullable: false),
                    LikedAtUtc = table.Column<DateTime>(nullable: false),
                    LoginId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectLike", x => x.ObjectLikeId);
                    table.ForeignKey(
                        name: "FK_ObjectLike_Logins_LoginId",
                        column: x => x.LoginId,
                        principalTable: "Logins",
                        principalColumn: "LoginId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectLike_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "OfferedObjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectLike_LoginId",
                table: "ObjectLike",
                column: "LoginId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectLike_ObjectId",
                table: "ObjectLike",
                column: "ObjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectLike");
        }
    }
}
