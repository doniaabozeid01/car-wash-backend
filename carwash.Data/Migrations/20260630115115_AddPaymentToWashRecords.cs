using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carwash.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentToWashRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "WashRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "WashRecords",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "WashRecords");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "WashRecords");
        }
    }
}
