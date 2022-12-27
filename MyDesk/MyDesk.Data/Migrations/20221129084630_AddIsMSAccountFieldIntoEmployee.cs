using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDesk.Data.Migrations
{
    public partial class AddIsMSAccountFieldIntoEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSSOAccount",
                table: "Employee",
                type: "bit",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE Employee SET IsSSOAccount = 1 WHERE isAdmin <> 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSSOAccount",
                table: "Employee");
        }
    }
}
