using Microsoft.EntityFrameworkCore.Migrations;

namespace Karma.Migrations
{
    public partial class AddColsMessaging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSeenByCreator",
                table: "Chat",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSeenByPostUser",
                table: "Chat",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PostUserId",
                table: "Chat",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSeenByCreator",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "IsSeenByPostUser",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "PostUserId",
                table: "Chat");
        }
    }
}
