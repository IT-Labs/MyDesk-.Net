using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inOfficeApplication.Data.Migrations
{
    public partial class DeskCategoriesMigrationWithoutEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoriesId",
                table: "Categories");

            migrationBuilder.AddColumn<bool>(
                name: "DoubleMonitor",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NearWindow",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SingleMonitor",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Unavailable",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoubleMonitor",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "NearWindow",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SingleMonitor",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Unavailable",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "CategoriesId",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
