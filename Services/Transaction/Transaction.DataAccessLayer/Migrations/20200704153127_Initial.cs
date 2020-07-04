using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Transaction.DataAccessLayer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OfferedObject",
                columns: table => new
                {
                    OfferedObjectId = table.Column<decimal>(nullable: false),
                    OriginalObjectId = table.Column<int>(nullable: false),
                    ShouldReturn = table.Column<bool>(nullable: false),
                    HourlyCharge = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferedObject", x => x.OfferedObjectId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    OriginalUserId = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    LoginId = table.Column<Guid>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    TokenId = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.LoginId);
                    table.ForeignKey(
                        name: "FK_Logins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectRegistrations",
                columns: table => new
                {
                    ObjectRegistrationId = table.Column<decimal>(nullable: false),
                    RegisteredAtUtc = table.Column<DateTime>(nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(nullable: false),
                    ShouldReturnItAfter = table.Column<TimeSpan>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    RecipientLoginId = table.Column<Guid>(nullable: false),
                    ObjectReceivingId = table.Column<decimal>(nullable: true),
                    ObjectId = table.Column<int>(nullable: false),
                    ObjectOfferedObjectId = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectRegistrations", x => x.ObjectRegistrationId);
                    table.ForeignKey(
                        name: "FK_ObjectRegistrations_OfferedObject_ObjectOfferedObjectId",
                        column: x => x.ObjectOfferedObjectId,
                        principalTable: "OfferedObject",
                        principalColumn: "OfferedObjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObjectRegistrations_Logins_RecipientLoginId",
                        column: x => x.RecipientLoginId,
                        principalTable: "Logins",
                        principalColumn: "LoginId");
                });

            migrationBuilder.CreateTable(
                name: "ObjectReceivings",
                columns: table => new
                {
                    ObjectReceivingId = table.Column<decimal>(nullable: false),
                    ObjectRegistrationId = table.Column<decimal>(nullable: false),
                    RecipientLoginId = table.Column<Guid>(nullable: false),
                    GiverLoginId = table.Column<Guid>(nullable: false),
                    ObjectReturningId = table.Column<decimal>(nullable: true),
                    ReceivedAtUtc = table.Column<DateTime>(nullable: false),
                    HourlyCharge = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectReceivings", x => x.ObjectReceivingId);
                    table.ForeignKey(
                        name: "FK_ObjectReceivings_Logins_GiverLoginId",
                        column: x => x.GiverLoginId,
                        principalTable: "Logins",
                        principalColumn: "LoginId");
                    table.ForeignKey(
                        name: "FK_ObjectReceivings_ObjectRegistrations_ObjectRegistrationId",
                        column: x => x.ObjectRegistrationId,
                        principalTable: "ObjectRegistrations",
                        principalColumn: "ObjectRegistrationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectReceivings_Logins_RecipientLoginId",
                        column: x => x.RecipientLoginId,
                        principalTable: "Logins",
                        principalColumn: "LoginId");
                });

            migrationBuilder.CreateTable(
                name: "ObjectReturnings",
                columns: table => new
                {
                    ObjectReturningId = table.Column<decimal>(nullable: false),
                    ReturnedAtUtc = table.Column<DateTime>(nullable: false),
                    LoaneeLoginId = table.Column<Guid>(nullable: false),
                    LoanerLoginId = table.Column<Guid>(nullable: false),
                    ObjectReceivingId = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectReturnings", x => x.ObjectReturningId);
                    table.ForeignKey(
                        name: "FK_ObjectReturnings_Logins_LoaneeLoginId",
                        column: x => x.LoaneeLoginId,
                        principalTable: "Logins",
                        principalColumn: "LoginId");
                    table.ForeignKey(
                        name: "FK_ObjectReturnings_Logins_LoanerLoginId",
                        column: x => x.LoanerLoginId,
                        principalTable: "Logins",
                        principalColumn: "LoginId");
                    table.ForeignKey(
                        name: "FK_ObjectReturnings_ObjectReceivings_ObjectReceivingId",
                        column: x => x.ObjectReceivingId,
                        principalTable: "ObjectReceivings",
                        principalColumn: "ObjectReceivingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logins_UserId",
                table: "Logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectReceivings_GiverLoginId",
                table: "ObjectReceivings",
                column: "GiverLoginId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectReceivings_ObjectRegistrationId",
                table: "ObjectReceivings",
                column: "ObjectRegistrationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectReceivings_RecipientLoginId",
                table: "ObjectReceivings",
                column: "RecipientLoginId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectRegistrations_ObjectOfferedObjectId",
                table: "ObjectRegistrations",
                column: "ObjectOfferedObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectRegistrations_RecipientLoginId",
                table: "ObjectRegistrations",
                column: "RecipientLoginId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectReturnings_LoaneeLoginId",
                table: "ObjectReturnings",
                column: "LoaneeLoginId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectReturnings_LoanerLoginId",
                table: "ObjectReturnings",
                column: "LoanerLoginId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectReturnings_ObjectReceivingId",
                table: "ObjectReturnings",
                column: "ObjectReceivingId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectReturnings");

            migrationBuilder.DropTable(
                name: "ObjectReceivings");

            migrationBuilder.DropTable(
                name: "ObjectRegistrations");

            migrationBuilder.DropTable(
                name: "OfferedObject");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
