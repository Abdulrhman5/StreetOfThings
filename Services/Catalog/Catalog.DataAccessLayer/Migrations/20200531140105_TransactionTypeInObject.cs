using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.DataAccessLayer.Migrations
{
    public partial class TransactionTypeInObject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLending",
                table: "Objects");

            migrationBuilder.AddColumn<int>(
                name: "CurrentTransactionType",
                table: "Objects",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentTransactionType",
                table: "Objects");

            migrationBuilder.AddColumn<bool>(
                name: "IsLending",
                table: "Objects",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
