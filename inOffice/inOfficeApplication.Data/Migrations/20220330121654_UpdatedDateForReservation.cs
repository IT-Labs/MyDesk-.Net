using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inOfficeApplication.Data.Migrations
{
    public partial class UpdatedDateForReservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Reservations",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Reservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$zSpN3p2ZO96j7F8kjWdiyOkxMNMiUxUssYEcs2XBMtAHu2xYHCJN2");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$nUTEwN0k9hCEU.qTcl5f8.oXrYeM2vwmkas8dR1szTI2nU2rL.E9O");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Reservations",
                newName: "Date");

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
        }
    }
}
