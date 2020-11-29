using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace Catalog.Infrastructure.Data.Migrations
{
    public partial class Initial : Migration
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
                    Description = table.Column<string>(nullable: true),
                    TagStatus = table.Column<int>(nullable: false),
                    PhotoId = table.Column<int>(nullable: false)
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
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "TagPhotos",
                columns: table => new
                {
                    PhotoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(nullable: true),
                    AdditionalInformation = table.Column<string>(nullable: true),
                    AddedAtUtc = table.Column<DateTime>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagPhotos", x => x.PhotoId);
                    table.ForeignKey(
                        name: "FK_TagPhotos_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    LoginId = table.Column<Guid>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    LoginLocation = table.Column<Point>(nullable: true),
                    LoggedAt = table.Column<DateTime>(nullable: false)
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
                    CurrentTransactionType = table.Column<int>(nullable: false),
                    ObjectStatus = table.Column<int>(nullable: false),
                    OwnerLoginId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objects", x => x.OfferedObjectId);
                    table.ForeignKey(
                        name: "FK_Objects_Logins_OwnerLoginId",
                        column: x => x.OwnerLoginId,
                        principalTable: "Logins",
                        principalColumn: "LoginId");
                });

            migrationBuilder.CreateTable(
                name: "ObjectComment",
                columns: table => new
                {
                    ObjectCommentId = table.Column<Guid>(nullable: false),
                    ObjectId = table.Column<int>(nullable: false),
                    LoginId = table.Column<Guid>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    AddedAtUtc = table.Column<DateTime>(nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectComment", x => x.ObjectCommentId);
                    table.ForeignKey(
                        name: "FK_ObjectComment_Logins_LoginId",
                        column: x => x.LoginId,
                        principalTable: "Logins",
                        principalColumn: "LoginId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectComment_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "OfferedObjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectImpressions",
                columns: table => new
                {
                    ObjectId = table.Column<int>(nullable: false),
                    LoginId = table.Column<Guid>(nullable: false),
                    ViewedAtUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectImpressions", x => new { x.ObjectId, x.LoginId, x.ViewedAtUtc });
                    table.ForeignKey(
                        name: "FK_ObjectImpressions_Logins_LoginId",
                        column: x => x.LoginId,
                        principalTable: "Logins",
                        principalColumn: "LoginId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectImpressions_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "OfferedObjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectLike",
                columns: table => new
                {
                    ObjectLikeId = table.Column<Guid>(nullable: false),
                    ObjectId = table.Column<int>(nullable: false),
                    LikedAtUtc = table.Column<DateTime>(nullable: false),
                    LoginId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectLike", x => x.ObjectLikeId);
                    table.ForeignKey(
                        name: "FK_ObjectLike_Logins_LoginId",
                        column: x => x.LoginId,
                        principalTable: "Logins",
                        principalColumn: "LoginId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectLike_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "OfferedObjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectPhotos",
                columns: table => new
                {
                    PhotoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(nullable: true),
                    AdditionalInformation = table.Column<string>(nullable: true),
                    AddedAtUtc = table.Column<DateTime>(nullable: false),
                    ObjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectPhotos", x => x.PhotoId);
                    table.ForeignKey(
                        name: "FK_ObjectPhotos_Objects_ObjectId",
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
                name: "ObjectView",
                columns: table => new
                {
                    ObjectId = table.Column<int>(nullable: false),
                    LoginId = table.Column<Guid>(nullable: false),
                    ViewedAtUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectView", x => new { x.ObjectId, x.LoginId, x.ViewedAtUtc });
                    table.ForeignKey(
                        name: "FK_ObjectView_Logins_LoginId",
                        column: x => x.LoginId,
                        principalTable: "Logins",
                        principalColumn: "LoginId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectView_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "OfferedObjectId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_Logins_UserId",
                table: "Logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectComment_LoginId",
                table: "ObjectComment",
                column: "LoginId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectComment_ObjectId",
                table: "ObjectComment",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectImpressions_LoginId",
                table: "ObjectImpressions",
                column: "LoginId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectLike_LoginId",
                table: "ObjectLike",
                column: "LoginId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectLike_ObjectId",
                table: "ObjectLike",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectPhotos_ObjectId",
                table: "ObjectPhotos",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Objects_OwnerLoginId",
                table: "Objects",
                column: "OwnerLoginId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectTags_TagId",
                table: "ObjectTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectView_LoginId",
                table: "ObjectView",
                column: "LoginId");

            migrationBuilder.CreateIndex(
                name: "IX_TagPhotos_TagId",
                table: "TagPhotos",
                column: "TagId",
                unique: true);

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
                name: "ObjectComment");

            migrationBuilder.DropTable(
                name: "ObjectImpressions");

            migrationBuilder.DropTable(
                name: "ObjectLike");

            migrationBuilder.DropTable(
                name: "ObjectPhotos");

            migrationBuilder.DropTable(
                name: "ObjectTags");

            migrationBuilder.DropTable(
                name: "ObjectView");

            migrationBuilder.DropTable(
                name: "TagPhotos");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Objects");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
