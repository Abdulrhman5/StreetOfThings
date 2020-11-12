using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Transaction.Service.Infrastructure.Migrations
{
    public partial class AddTransactionToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionToken",
                columns: table => new
                {
                    TransactionTokenId = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    IssuedAtUtc = table.Column<DateTime>(nullable: false),
                    UseAfterUtc = table.Column<DateTime>(nullable: false),
                    UseBeforeUtc = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    ReceivingId = table.Column<Guid>(nullable: true),
                    RegistrationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionToken", x => x.TransactionTokenId);
                    table.ForeignKey(
                        name: "FK_TransactionToken_ObjectReceivings_ReceivingId",
                        column: x => x.ReceivingId,
                        principalTable: "ObjectReceivings",
                        principalColumn: "ObjectReceivingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionToken_ObjectRegistrations_RegistrationId",
                        column: x => x.RegistrationId,
                        principalTable: "ObjectRegistrations",
                        principalColumn: "ObjectRegistrationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionToken_ReceivingId",
                table: "TransactionToken",
                column: "ReceivingId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionToken_RegistrationId",
                table: "TransactionToken",
                column: "RegistrationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionToken");
        }
    }
}
