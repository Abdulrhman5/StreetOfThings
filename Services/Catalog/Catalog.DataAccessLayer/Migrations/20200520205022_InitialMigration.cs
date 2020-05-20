using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.DataAccessLayer.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    OriginalUserId = table.Column<string>(nullable: true)
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
                name: "Objects",
                columns: table => new
                {
                    OfferedObjectId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PublishedAt = table.Column<DateTime>(nullable: false),
                    EndsAt = table.Column<DateTime>(nullable: true),
                    IsLending = table.Column<bool>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objects", x => x.OfferedObjectId);
                    table.ForeignKey(
                        name: "FK_Objects_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectsLoanProperties",
                columns: table => new
                {
                    ObjectId = table.Column<int>(nullable: false)
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
                name: "ObjectTags",
                columns: table => new
                {
                    ObjectId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectTags", x => new { x.ObjectId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ObjectTags_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "OfferedObjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectLoans",
                columns: table => new
                {
                    ObjectLoanId = table.Column<Guid>(nullable: false),
                    LoanedAt = table.Column<DateTime>(nullable: false),
                    LoanEndAt = table.Column<DateTime>(nullable: true),
                    Rating = table.Column<float>(nullable: true),
                    LoginId = table.Column<Guid>(nullable: false),
                    ObjectLoanPropertiesObjectId = table.Column<int>(nullable: true)
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
                name: "IX_Logins_UserId",
                table: "Logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectLoans_LoginId",
                table: "ObjectLoans",
                column: "LoginId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectLoans_ObjectLoanPropertiesObjectId",
                table: "ObjectLoans",
                column: "ObjectLoanPropertiesObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Objects_OwnerId",
                table: "Objects",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectTags_TagId",
                table: "ObjectTags",
                column: "TagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectLoans");

            migrationBuilder.DropTable(
                name: "ObjectTags");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "ObjectsLoanProperties");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Objects");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
