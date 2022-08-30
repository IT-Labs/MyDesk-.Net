using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inOfficeApplication.Data.Migrations
{
    public partial class SeedAdminData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "Id", "Email", "FirstName", "IsAdmin", "IsDeleted", "JobTitle", "LastName", "Password" },
                values: new object[] { 1, "admin@it-labs.com", "Admin", true, false, "admin", "Employee", "$2a$11$wdjhCrEA60JEqot3lW4yguYBIgmyZ/IWpnS6VUgjDOlK2VORFpddi" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
