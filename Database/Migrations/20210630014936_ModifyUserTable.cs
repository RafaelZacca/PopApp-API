using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class ModifyUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "Users",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "DeviceType",
                table: "Users",
                newName: "Password");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "DeviceType");
        }
    }
}
