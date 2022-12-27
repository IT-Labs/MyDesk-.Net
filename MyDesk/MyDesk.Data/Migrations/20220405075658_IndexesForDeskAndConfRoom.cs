using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDesk.Data.Migrations
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndexForOffice",
                table: "Desks");

            migrationBuilder.DropColumn(
                name: "IndexForOffice",
                table: "ConferenceRooms");
        }
    }
}
