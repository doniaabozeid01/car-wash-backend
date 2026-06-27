using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carwash.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeQrCodeOptionalForCashier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_QrCode",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "QrCode",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_QrCode",
                table: "AspNetUsers",
                column: "QrCode",
                unique: true,
                filter: "[QrCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_QrCode",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "QrCode",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_QrCode",
                table: "AspNetUsers",
                column: "QrCode",
                unique: true);
        }
    }
}
