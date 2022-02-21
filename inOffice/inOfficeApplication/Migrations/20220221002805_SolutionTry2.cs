using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inOfficeApplication.Migrations
{
    public partial class SolutionTry2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modes_ConferenceRooms_ConferenceRoomId",
                table: "Modes");

            migrationBuilder.DropIndex(
                name: "IX_Modes_ConferenceRoomId",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "ConferenceRoomId",
                table: "Modes");

            migrationBuilder.CreateTable(
                name: "ConferenceRoomMode",
                columns: table => new
                {
                    ConferenceRoomId = table.Column<int>(type: "int", nullable: false),
                    ModeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConferenceRoomMode", x => new { x.ConferenceRoomId, x.ModeId });
                    table.ForeignKey(
                        name: "FK_ConferenceRoomMode_ConferenceRooms_ConferenceRoomId",
                        column: x => x.ConferenceRoomId,
                        principalTable: "ConferenceRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConferenceRoomMode_Modes_ModeId",
                        column: x => x.ModeId,
                        principalTable: "Modes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceRoomMode_ModeId",
                table: "ConferenceRoomMode",
                column: "ModeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConferenceRoomMode");

            migrationBuilder.AddColumn<int>(
                name: "ConferenceRoomId",
                table: "Modes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modes_ConferenceRoomId",
                table: "Modes",
                column: "ConferenceRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modes_ConferenceRooms_ConferenceRoomId",
                table: "Modes",
                column: "ConferenceRoomId",
                principalTable: "ConferenceRooms",
                principalColumn: "Id");
        }
    }
}
