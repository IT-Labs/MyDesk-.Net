using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDesk.Data.Migrations
{
    public partial class SoftDeleteUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeskModes",
                table: "DeskModes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConferenceRoomModes",
                table: "ConferenceRoomModes");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Offices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Modes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Desks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DeskModes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DeskModes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ConferenceRooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ConferenceRoomModes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ConferenceRoomModes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Admins",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeskModes",
                table: "DeskModes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConferenceRoomModes",
                table: "ConferenceRoomModes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DeskModes_DeskId",
                table: "DeskModes",
                column: "DeskId");

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceRoomModes_ConferenceRoomId",
                table: "ConferenceRoomModes",
                column: "ConferenceRoomId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeskModes",
                table: "DeskModes");

            migrationBuilder.DropIndex(
                name: "IX_DeskModes_DeskId",
                table: "DeskModes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConferenceRoomModes",
                table: "ConferenceRoomModes");

            migrationBuilder.DropIndex(
                name: "IX_ConferenceRoomModes_ConferenceRoomId",
                table: "ConferenceRoomModes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Desks");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DeskModes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DeskModes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ConferenceRooms");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ConferenceRoomModes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ConferenceRoomModes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Admins");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeskModes",
                table: "DeskModes",
                columns: new[] { "DeskId", "ModeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConferenceRoomModes",
                table: "ConferenceRoomModes",
                columns: new[] { "ConferenceRoomId", "ModeId" });
        }
    }
}
