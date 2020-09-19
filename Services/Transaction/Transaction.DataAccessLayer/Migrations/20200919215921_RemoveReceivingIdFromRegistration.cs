using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Transaction.DataAccessLayer.Migrations
{
    public partial class RemoveReceivingIdFromRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObjectReceivingId",
                table: "ObjectRegistrations");

            migrationBuilder.DropColumn(
                name: "ObjectReturningId",
                table: "ObjectReceivings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ObjectReceivingId",
                table: "ObjectRegistrations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ObjectReturningId",
                table: "ObjectReceivings",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
