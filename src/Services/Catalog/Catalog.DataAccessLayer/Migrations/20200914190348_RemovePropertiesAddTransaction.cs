using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.DataAccessLayer.Migrations
{
    public partial class RemovePropertiesAddTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectFreeProperties");

            migrationBuilder.DropTable(
                name: "ObjectLoans");

            migrationBuilder.DropTable(
                name: "ObjectsLoanProperties");

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(nullable: false),
                    RegisteredAtUtc = table.Column<DateTime>(nullable: false),
                    ReceivingId = table.Column<Guid>(nullable: true),
                    ReceivedAtUtc = table.Column<DateTime>(nullable: true),
                    ReturnId = table.Column<Guid>(nullable: true),
                    ReturnedAtUtc = table.Column<DateTime>(nullable: true),
                    Rating = table.Column<float>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ReceipientId = table.Column<Guid>(nullable: false),
                    ObjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transaction_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "OfferedObjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transaction_Users_ReceipientId",
                        column: x => x.ReceipientId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ObjectId",
                table: "Transaction",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ReceipientId",
                table: "Transaction",
                column: "ReceipientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.CreateTable(
                name: "ObjectFreeProperties",
                columns: table => new
                {
                    ObjectId = table.Column<int>(type: "int", nullable: false),
                    OfferedFreeAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TakenAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TakerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectFreeProperties", x => x.ObjectId);
                    table.ForeignKey(
                        name: "FK_ObjectFreeProperties_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "OfferedObjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectFreeProperties_Users_TakerId",
                        column: x => x.TakerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ObjectsLoanProperties",
                columns: table => new
                {
                    ObjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectsLoanProperties", x => x.ObjectId);
                    table.ForeignKey(
                        name: "FK_ObjectsLoanProperties_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "OfferedObjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectLoans",
                columns: table => new
                {
                    ObjectLoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoanEndAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoanedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoginId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObjectLoanPropertiesObjectId = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectLoans", x => x.ObjectLoanId);
                    table.ForeignKey(
                        name: "FK_ObjectLoans_Logins_LoginId",
                        column: x => x.LoginId,
                        principalTable: "Logins",
                        principalColumn: "LoginId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectLoans_ObjectsLoanProperties_ObjectLoanPropertiesObjectId",
                        column: x => x.ObjectLoanPropertiesObjectId,
                        principalTable: "ObjectsLoanProperties",
                        principalColumn: "ObjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectFreeProperties_TakerId",
                table: "ObjectFreeProperties",
                column: "TakerId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectLoans_LoginId",
                table: "ObjectLoans",
                column: "LoginId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectLoans_ObjectLoanPropertiesObjectId",
                table: "ObjectLoans",
                column: "ObjectLoanPropertiesObjectId");
        }
    }
}
