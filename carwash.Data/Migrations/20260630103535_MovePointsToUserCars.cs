using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carwash.Data.Migrations
{
    /// <inheritdoc />
    public partial class MovePointsToUserCars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "UserCars",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE uc
                SET uc.Points = u.Points
                FROM UserCars uc
                INNER JOIN (
                    SELECT UserId, MIN(Id) AS CarId
                    FROM UserCars
                    GROUP BY UserId
                ) first_car ON uc.Id = first_car.CarId
                INNER JOIN AspNetUsers u ON u.Id = first_car.UserId
                WHERE u.Points > 0
                """);

            migrationBuilder.DropColumn(
                name: "Points",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE u
                SET u.Points = car_totals.TotalPoints
                FROM AspNetUsers u
                INNER JOIN (
                    SELECT UserId, SUM(Points) AS TotalPoints
                    FROM UserCars
                    GROUP BY UserId
                ) car_totals ON car_totals.UserId = u.Id
                """);

            migrationBuilder.DropColumn(
                name: "Points",
                table: "UserCars");
        }
    }
}
