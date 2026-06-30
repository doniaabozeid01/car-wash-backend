using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carwash.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWashRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WashRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserFullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    PlateNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CarType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CarSize = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    WashServiceId = table.Column<int>(type: "int", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PointsChange = table.Column<int>(type: "int", nullable: false),
                    CarPointsAfter = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WashRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WashRecords_CarId_CreatedAt",
                table: "WashRecords",
                columns: new[] { "CarId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WashRecords_CreatedAt",
                table: "WashRecords",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WashRecords_UserId_CreatedAt",
                table: "WashRecords",
                columns: new[] { "UserId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WashRecords");
        }
    }
}
