using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carwash.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBilingualWashServiceNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "WashServices",
                newName: "NameAr");

            migrationBuilder.RenameIndex(
                name: "IX_WashServices_Name",
                table: "WashServices",
                newName: "IX_WashServices_NameAr");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "WashServices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.RenameColumn(
                name: "ServiceName",
                table: "WashRecords",
                newName: "ServiceNameAr");

            migrationBuilder.AddColumn<string>(
                name: "ServiceNameEn",
                table: "WashRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "WashServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "NameEn",
                value: "Normal Wash");

            migrationBuilder.UpdateData(
                table: "WashServices",
                keyColumn: "Id",
                keyValue: 2,
                column: "NameEn",
                value: "Premium Wash");

            migrationBuilder.UpdateData(
                table: "WashServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "NameEn",
                value: "Free Wash");

            migrationBuilder.Sql("""
                UPDATE WashRecords
                SET ServiceNameEn = CASE ServiceNameAr
                    WHEN N'غسلة عادية' THEN N'Normal Wash'
                    WHEN N'غسلة مميزة' THEN N'Premium Wash'
                    WHEN N'غسلة مجانية' THEN N'Free Wash'
                    ELSE ServiceNameAr
                END
                """);

            migrationBuilder.CreateIndex(
                name: "IX_WashServices_NameEn",
                table: "WashServices",
                column: "NameEn",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WashServices_NameEn",
                table: "WashServices");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "WashServices");

            migrationBuilder.DropColumn(
                name: "ServiceNameEn",
                table: "WashRecords");

            migrationBuilder.RenameColumn(
                name: "NameAr",
                table: "WashServices",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_WashServices_NameAr",
                table: "WashServices",
                newName: "IX_WashServices_Name");

            migrationBuilder.RenameColumn(
                name: "ServiceNameAr",
                table: "WashRecords",
                newName: "ServiceName");
        }
    }
}
