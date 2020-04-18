using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace DataAccessLayer.Migrations
{
    public partial class AddLogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Login",
                columns: table => new
                {
                    LoginId = table.Column<Guid>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    IsValid = table.Column<bool>(nullable: false),
                    IPAddress = table.Column<string>(nullable: true),
                    ClientAgent = table.Column<string>(nullable: true),
                    AdditionalInformation = table.Column<string>(nullable: true),
                    LoggedAt = table.Column<DateTime>(nullable: false),
                    LoggedOutAt = table.Column<DateTime>(nullable: true),
                    LoginLocation = table.Column<Point>(nullable: true),
                    Imei = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Login", x => x.LoginId);
                    table.ForeignKey(
                        name: "FK_Login_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Login_UserId",
                table: "Login",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Login");
        }
    }
}
