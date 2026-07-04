using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carwash.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPointsCampaigns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PointsCampaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PointsAmount = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedByCashierId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointsCampaigns_AspNetUsers_CreatedByCashierId",
                        column: x => x.CreatedByCashierId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserCampaignReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PointsAdded = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCampaignReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCampaignReceipts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCampaignReceipts_PointsCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "PointsCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointsCampaigns_CreatedAt",
                table: "PointsCampaigns",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PointsCampaigns_CreatedByCashierId",
                table: "PointsCampaigns",
                column: "CreatedByCashierId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCampaignReceipts_CampaignId",
                table: "UserCampaignReceipts",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCampaignReceipts_UserId_CampaignId",
                table: "UserCampaignReceipts",
                columns: new[] { "UserId", "CampaignId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCampaignReceipts_UserId_IsRead",
                table: "UserCampaignReceipts",
                columns: new[] { "UserId", "IsRead" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCampaignReceipts");

            migrationBuilder.DropTable(
                name: "PointsCampaigns");
        }
    }
}
