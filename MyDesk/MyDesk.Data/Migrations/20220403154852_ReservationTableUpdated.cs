using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDesk.Data.Migrations
{
    public partial class ReservationTableUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConferenceRoomId",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeskId",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewId",
                table: "Reservations",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConferenceRoomId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DeskId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReviewId",
                table: "Reservations");
        }
    }
}
