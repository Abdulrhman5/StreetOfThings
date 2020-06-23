using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.DataAccessLayer.Migrations
{
    public partial class ObjectRelatedToLogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objects_Users_OwnerId",
                table: "Objects");

            migrationBuilder.DropIndex(
                name: "IX_Objects_OwnerId",
                table: "Objects");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Objects");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerLoginId",
                table: "Objects",
                nullable: false,
                defaultValue: new Guid("d199b4f6-2ea0-4334-a9b8-0604492748e3"));

            migrationBuilder.CreateIndex(
                name: "IX_Objects_OwnerLoginId",
                table: "Objects",
                column: "OwnerLoginId");

            migrationBuilder.AddForeignKey(
                name: "FK_Objects_Logins_OwnerLoginId",
                table: "Objects",
                column: "OwnerLoginId",
                principalTable: "Logins",
                principalColumn: "LoginId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objects_Logins_OwnerLoginId",
                table: "Objects");

            migrationBuilder.DropIndex(
                name: "IX_Objects_OwnerLoginId",
                table: "Objects");

            migrationBuilder.DropColumn(
                name: "OwnerLoginId",
                table: "Objects");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Objects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("e67e845b-3cf3-42af-b5eb-40ad83bf3f21"));

            migrationBuilder.CreateIndex(
                name: "IX_Objects_OwnerId",
                table: "Objects",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Objects_Users_OwnerId",
                table: "Objects",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
