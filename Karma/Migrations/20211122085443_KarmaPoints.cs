using Microsoft.EntityFrameworkCore.Migrations;

namespace Karma.Migrations
{
    public partial class KarmaPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KarmaPoints",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KarmaPoints",
                table: "AspNetUsers");
        }
    }
}
