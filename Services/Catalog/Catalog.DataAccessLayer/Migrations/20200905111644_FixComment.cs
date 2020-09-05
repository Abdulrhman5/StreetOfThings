using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.DataAccessLayer.Migrations
{
    public partial class FixComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectComment_Objects_OfferedObjectId",
                table: "ObjectComment");

            migrationBuilder.DropIndex(
                name: "IX_ObjectComment_OfferedObjectId",
                table: "ObjectComment");

            migrationBuilder.DropColumn(
                name: "OfferedObjectId",
                table: "ObjectComment");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectComment_ObjectId",
                table: "ObjectComment",
                column: "ObjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectComment_Objects_ObjectId",
                table: "ObjectComment",
                column: "ObjectId",
                principalTable: "Objects",
                principalColumn: "OfferedObjectId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectComment_Objects_ObjectId",
                table: "ObjectComment");

            migrationBuilder.DropIndex(
                name: "IX_ObjectComment_ObjectId",
                table: "ObjectComment");

            migrationBuilder.AddColumn<int>(
                name: "OfferedObjectId",
                table: "ObjectComment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectComment_OfferedObjectId",
                table: "ObjectComment",
                column: "OfferedObjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectComment_Objects_OfferedObjectId",
                table: "ObjectComment",
                column: "OfferedObjectId",
                principalTable: "Objects",
                principalColumn: "OfferedObjectId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
