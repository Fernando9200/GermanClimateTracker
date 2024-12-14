using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWeatherIcon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeelsLike",
                table: "WeatherData");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "WeatherData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "WeatherData");

            migrationBuilder.AddColumn<double>(
                name: "FeelsLike",
                table: "WeatherData",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
