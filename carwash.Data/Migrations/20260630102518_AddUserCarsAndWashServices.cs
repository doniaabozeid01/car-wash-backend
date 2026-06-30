using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace carwash.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCarsAndWashServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CarType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlateNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCars_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WashServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WashServices", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "WashServices",
                columns: new[] { "Id", "Name", "Points" },
                values: new object[,]
                {
                    { 1, "غسلة عادية", 30 },
                    { 2, "غسلة مميزة", 50 },
                    { 3, "غسلة مجانية", -250 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCars_UserId_PlateNumber",
                table: "UserCars",
                columns: new[] { "UserId", "PlateNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WashServices_Name",
                table: "WashServices",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCars");

            migrationBuilder.DropTable(
                name: "WashServices");
        }
    }
}
