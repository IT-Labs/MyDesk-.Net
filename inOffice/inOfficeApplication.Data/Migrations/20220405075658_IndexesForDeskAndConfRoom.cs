using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inOfficeApplication.Data.Migrations
{
    public partial class IndexesForDeskAndConfRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IndexForOffice",
                table: "Desks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IndexForOffice",
                table: "ConferenceRooms",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$WRSQeZssYnL7GJonfkzQ0eV0fBYLbHb0iUoHhocbVmaKt/F8LcDFS");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$YEnLMN.02kcZDl7zBpwlMuHkgOiVipK.GIISxMRkBIjGmfXJaT4zW");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndexForOffice",
                table: "Desks");

            migrationBuilder.DropColumn(
                name: "IndexForOffice",
                table: "ConferenceRooms");

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
    }
}
