using Microsoft.EntityFrameworkCore.Migrations;

namespace Karma.Migrations
{
    public partial class RemoveColsFromCharity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressesPath",
                table: "Charity");

            migrationBuilder.DropColumn(
                name: "ItemTypePath",
                table: "Charity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressesPath",
                table: "Charity",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ItemTypePath",
                table: "Charity",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
