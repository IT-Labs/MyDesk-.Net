using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inOfficeApplication.Data.Migrations
{
    public partial class SeedingAdminAndUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Password" },
                values: new object[] { 2, "admin@inoffice.com", "Nekoj Admin", "Prezime Admin", "$2a$11$.wPZU45XwT68GfaUcSfY6euymKTyTiesitdH8xVG3WGZV56rb6ZoC" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Email", "FirstName", "JobTitle", "LastName", "Password" },
                values: new object[] { 1, "user@inoffice.com", "Nekoj Employee", null, "Prezime Employee", "$2a$11$WGfOjQvLqp3rhBlerZS3duvq2/MSqUJaZWW45Xic4mnbylUVZIBLS" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
