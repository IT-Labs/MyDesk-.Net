using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inOfficeApplication.Data.Migrations
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

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$xY6blM17V4Y2vN4zy5OCKO6hSregSe./SeCQFMZwdoRbjLxvZnk9.");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$t.j5k9AizO/U9.khNZsQgugF5v8K5zQzXyJjU.gU.8NhZ6xZofNEm");
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

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$IfNiAMlgRgvqOyW5mEfvK.Cy14MdL29J8HMCfUpOwObS7UUB40cLG");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$QXL4GkT7FXcGDnLpMdoMteQa3Ku6hF91B5745Jn1kps4zTrjiWORi");
        }
    }
}
