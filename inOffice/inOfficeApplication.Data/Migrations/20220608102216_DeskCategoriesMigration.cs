using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inOfficeApplication.Data.Migrations
{
    public partial class DeskCategoriesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategorieId",
                table: "Desks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeskId = table.Column<int>(type: "int", nullable: false),
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeskCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeskId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeskCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeskCategories_Categories_DeskId",
                        column: x => x.DeskId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DeskCategories_Desks_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Desks",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_DeskCategories_CategoryId",
                table: "DeskCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DeskCategories_DeskId",
                table: "DeskCategories",
                column: "DeskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeskCategories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropColumn(
                name: "CategorieId",
                table: "Desks");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$gR9BmBM4Egj.OCQK3xU.iOF5SuYrG7xQPY0Pf/WEFjkVvU8Uplur2");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$SpHiYycg6TPqsEYp8takzewxiXrRrM56a/I9NZWdkcg1BvNvEbruK");
        }
    }
}
