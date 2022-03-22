using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inOfficeApplication.Data.Migrations
{
    public partial class DeskInsertNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Desks_Reservations_ReservationId",
                table: "Desks");

            migrationBuilder.DropIndex(
                name: "IX_Desks_ReservationId",
                table: "Desks");

            migrationBuilder.AlterColumn<int>(
                name: "ReservationId",
                table: "Desks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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
                name: "IX_Desks_ReservationId",
                table: "Desks",
                column: "ReservationId",
                unique: true,
                filter: "[ReservationId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Desks_Reservations_ReservationId",
                table: "Desks",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Desks_Reservations_ReservationId",
                table: "Desks");

            migrationBuilder.DropIndex(
                name: "IX_Desks_ReservationId",
                table: "Desks");

            migrationBuilder.AlterColumn<int>(
                name: "ReservationId",
                table: "Desks",
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
                value: "$2a$11$5UhL1eBw3qzxpO5bwrYx4e.DFhXdyl7/i1yaXOPD/IY9G1HZSqP1m");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$esLRp7lvteptwuQC6NRCw.lGa60QwjgzQN4D6majsWFcmNtFNagdG");

            migrationBuilder.CreateIndex(
                name: "IX_Desks_ReservationId",
                table: "Desks",
                column: "ReservationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Desks_Reservations_ReservationId",
                table: "Desks",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
