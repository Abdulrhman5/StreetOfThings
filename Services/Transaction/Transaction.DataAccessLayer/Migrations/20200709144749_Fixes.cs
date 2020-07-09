using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Transaction.DataAccessLayer.Migrations
{
    public partial class Fixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectRegistrations_OfferedObject_ObjectOfferedObjectId",
                table: "ObjectRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_ObjectRegistrations_ObjectOfferedObjectId",
                table: "ObjectRegistrations");

            migrationBuilder.DropColumn(
                name: "ObjectOfferedObjectId",
                table: "ObjectRegistrations");

            migrationBuilder.AlterColumn<decimal>(
                name: "OfferedObjectId",
                table: "OfferedObject",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerUserId",
                table: "OfferedObject",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<decimal>(
                name: "ObjectReturningId",
                table: "ObjectReturnings",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ShouldReturnItAfter",
                table: "ObjectRegistrations",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<decimal>(
                name: "ObjectId",
                table: "ObjectRegistrations",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "ObjectRegistrationId",
                table: "ObjectRegistrations",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<decimal>(
                name: "ObjectReceivingId",
                table: "ObjectReceivings",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateIndex(
                name: "IX_OfferedObject_OwnerUserId",
                table: "OfferedObject",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectRegistrations_ObjectId",
                table: "ObjectRegistrations",
                column: "ObjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectRegistrations_OfferedObject_ObjectId",
                table: "ObjectRegistrations",
                column: "ObjectId",
                principalTable: "OfferedObject",
                principalColumn: "OfferedObjectId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferedObject_Users_OwnerUserId",
                table: "OfferedObject",
                column: "OwnerUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectRegistrations_OfferedObject_ObjectId",
                table: "ObjectRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferedObject_Users_OwnerUserId",
                table: "OfferedObject");

            migrationBuilder.DropIndex(
                name: "IX_OfferedObject_OwnerUserId",
                table: "OfferedObject");

            migrationBuilder.DropIndex(
                name: "IX_ObjectRegistrations_ObjectId",
                table: "ObjectRegistrations");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "OfferedObject");

            migrationBuilder.AlterColumn<decimal>(
                name: "OfferedObjectId",
                table: "OfferedObject",
                type: "decimal(20,0)",
                nullable: false,
                oldClrType: typeof(decimal))
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<decimal>(
                name: "ObjectReturningId",
                table: "ObjectReturnings",
                type: "decimal(20,0)",
                nullable: false,
                oldClrType: typeof(decimal))
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ShouldReturnItAfter",
                table: "ObjectRegistrations",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ObjectId",
                table: "ObjectRegistrations",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "ObjectRegistrationId",
                table: "ObjectRegistrations",
                type: "decimal(20,0)",
                nullable: false,
                oldClrType: typeof(decimal))
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<decimal>(
                name: "ObjectOfferedObjectId",
                table: "ObjectRegistrations",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ObjectReceivingId",
                table: "ObjectReceivings",
                type: "decimal(20,0)",
                nullable: false,
                oldClrType: typeof(decimal))
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectRegistrations_ObjectOfferedObjectId",
                table: "ObjectRegistrations",
                column: "ObjectOfferedObjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectRegistrations_OfferedObject_ObjectOfferedObjectId",
                table: "ObjectRegistrations",
                column: "ObjectOfferedObjectId",
                principalTable: "OfferedObject",
                principalColumn: "OfferedObjectId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
