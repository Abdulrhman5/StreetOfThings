using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.DataAccessLayer.Migrations
{
    public partial class AddTagPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TagPhoto",
                columns: table => new
                {
                    TagId = table.Column<int>(nullable: false),
                    FilePath = table.Column<string>(nullable: true),
                    AdditionalInformation = table.Column<string>(nullable: true),
                    AddedAtUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagPhoto", x => x.TagId);
                    table.ForeignKey(
                        name: "FK_TagPhoto_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagPhoto");
        }
    }
}
