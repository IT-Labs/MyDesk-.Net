using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inOfficeApplication.Data.Migrations
{
    public partial class LazyLoadingData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$IfNiAMlgRgvqOyW5mEfvK.Cy14MdL29J8HMCfUpOwObS7UUB40cLG");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$QXL4GkT7FXcGDnLpMdoMteQa3Ku6hF91B5745Jn1kps4zTrjiWORi");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$zSpN3p2ZO96j7F8kjWdiyOkxMNMiUxUssYEcs2XBMtAHu2xYHCJN2");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$nUTEwN0k9hCEU.qTcl5f8.oXrYeM2vwmkas8dR1szTI2nU2rL.E9O");
        }
    }
}
