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

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$GbYZXsuaza80dC4zNp7T5u6OHgCslHgWPrj/0W8ANhxOvWNNFNTC2");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$BBZYzOYelQJQgDyifH1DkuyuVmUFCZtuLjNLXNPJda6a/hLMVav0m");
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

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$VJYnT7RraGX22NQq8QGhKexacpmBokvQ2LY4JnzBAD2FJoC7q1Yp6");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$fb9wKLniDjfRQY2GWWsBeOOqCyn.yeAP7NhoiI4D5KayF391czMS6");
        }
    }
}
