using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightData.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTripTypeAndDefaultCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pasengers",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "TripType",
                table: "Flights");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Pasengers",
                table: "Flights",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TripType",
                table: "Flights",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
