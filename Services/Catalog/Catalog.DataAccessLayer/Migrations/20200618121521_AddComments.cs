using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.DataAccessLayer.Migrations
{
    public partial class AddComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObjectComment",
                columns: table => new
                {
                    ObjectCommentId = table.Column<Guid>(nullable: false),
                    ObjectId = table.Column<int>(nullable: false),
                    LoginId = table.Column<Guid>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    AddedAtUtc = table.Column<DateTime>(nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(nullable: false),
                    OfferedObjectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectComment", x => x.ObjectCommentId);
                    table.ForeignKey(
                        name: "FK_ObjectComment_Logins_LoginId",
                        column: x => x.LoginId,
                        principalTable: "Logins",
                        principalColumn: "LoginId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectComment_Objects_OfferedObjectId",
                        column: x => x.OfferedObjectId,
                        principalTable: "Objects",
                        principalColumn: "OfferedObjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectComment_LoginId",
                table: "ObjectComment",
                column: "LoginId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectComment_OfferedObjectId",
                table: "ObjectComment",
                column: "OfferedObjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectComment");
        }
    }
}
