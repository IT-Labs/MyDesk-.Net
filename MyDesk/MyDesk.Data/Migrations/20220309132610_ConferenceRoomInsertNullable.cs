using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDesk.Data.Migrations
{
    public partial class ConferenceRoomInsertNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConferenceRooms_Reservations_ReservationId",
                table: "ConferenceRooms");

            migrationBuilder.DropIndex(
                name: "IX_ConferenceRooms_ReservationId",
                table: "ConferenceRooms");

            migrationBuilder.AlterColumn<int>(
                name: "ReservationId",
                table: "ConferenceRooms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceRooms_ReservationId",
                table: "ConferenceRooms",
                column: "ReservationId",
                unique: true,
                filter: "[ReservationId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ConferenceRooms_Reservations_ReservationId",
                table: "ConferenceRooms",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConferenceRooms_Reservations_ReservationId",
                table: "ConferenceRooms");

            migrationBuilder.DropIndex(
                name: "IX_ConferenceRooms_ReservationId",
                table: "ConferenceRooms");

            migrationBuilder.AlterColumn<int>(
                name: "ReservationId",
                table: "ConferenceRooms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceRooms_ReservationId",
                table: "ConferenceRooms",
                column: "ReservationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConferenceRooms_Reservations_ReservationId",
                table: "ConferenceRooms",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
