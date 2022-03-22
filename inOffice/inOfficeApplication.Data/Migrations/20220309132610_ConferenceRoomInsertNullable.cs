using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inOfficeApplication.Data.Migrations
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

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$rYujnzLxeudGRY7bOqMRaewrcvzJOxk13U3ZaTaEw7tQARQNOTQpm");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$905PCW6R/YCUhLO7iT2CIeukMZA0QJQt6Ur55UcQPxLxoGoum2yu2");

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

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$sk6hPno222fBITblqzURhuu92w.BpIDFgcG6Zjx4u.Ak6tRFC19m2");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$vyXLEo.Rk3ETcZfn2zVVluQCVJEPqFjt6wX622Wgg7WRNQjHBSkRK");

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
