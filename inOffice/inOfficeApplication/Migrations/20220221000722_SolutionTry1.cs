using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inOfficeApplication.Migrations
{
    public partial class SolutionTry1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConferenceRoomId",
                table: "Modes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeskModes",
                columns: table => new
                {
                    DeskId = table.Column<int>(type: "int", nullable: false),
                    ModeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeskModes", x => new { x.DeskId, x.ModeId });
                    table.ForeignKey(
                        name: "FK_DeskModes_Desks_DeskId",
                        column: x => x.DeskId,
                        principalTable: "Desks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DeskModes_Modes_ModeId",
                        column: x => x.ModeId,
                        principalTable: "Modes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Modes_ConferenceRoomId",
                table: "Modes",
                column: "ConferenceRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_DeskModes_ModeId",
                table: "DeskModes",
                column: "ModeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modes_ConferenceRooms_ConferenceRoomId",
                table: "Modes",
                column: "ConferenceRoomId",
                principalTable: "ConferenceRooms",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modes_ConferenceRooms_ConferenceRoomId",
                table: "Modes");

            migrationBuilder.DropTable(
                name: "DeskModes");

            migrationBuilder.DropIndex(
                name: "IX_Modes_ConferenceRoomId",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "ConferenceRoomId",
                table: "Modes");
        }
    }
}
