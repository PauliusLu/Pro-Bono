using Microsoft.EntityFrameworkCore.Migrations;

namespace Karma.Migrations
{
    public partial class AddCharityIdCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<int>(
                name: "CharityId",
                table: "AspNetUserRoles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUserRoles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CharityId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUserRoles");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetRoles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
