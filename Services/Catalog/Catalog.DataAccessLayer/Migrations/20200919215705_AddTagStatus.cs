using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.DataAccessLayer.Migrations
{
    public partial class AddTagStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TagStatus",
                table: "Tags",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagStatus",
                table: "Tags");
        }
    }
}
