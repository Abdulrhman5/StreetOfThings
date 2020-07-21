using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Transaction.DataAccessLayer.Migrations
{
    public partial class AddLoginToToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IssuerLoginId",
                table: "TransactionToken",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TransactionToken_IssuerLoginId",
                table: "TransactionToken",
                column: "IssuerLoginId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionToken_Logins_IssuerLoginId",
                table: "TransactionToken",
                column: "IssuerLoginId",
                principalTable: "Logins",
                principalColumn: "LoginId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionToken_Logins_IssuerLoginId",
                table: "TransactionToken");

            migrationBuilder.DropIndex(
                name: "IX_TransactionToken_IssuerLoginId",
                table: "TransactionToken");

            migrationBuilder.DropColumn(
                name: "IssuerLoginId",
                table: "TransactionToken");
        }
    }
}
