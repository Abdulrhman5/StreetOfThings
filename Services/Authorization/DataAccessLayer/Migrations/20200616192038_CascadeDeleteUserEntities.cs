using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class CascadeDeleteUserEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Login_AspNetUsers_UserId",
                table: "Login");

            migrationBuilder.AddForeignKey(
                name: "FK_Login_AspNetUsers_UserId",
                table: "Login",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Login_AspNetUsers_UserId",
                table: "Login");

            migrationBuilder.AddForeignKey(
                name: "FK_Login_AspNetUsers_UserId",
                table: "Login",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
