using Microsoft.EntityFrameworkCore.Migrations;

namespace Karma.Migrations
{
    public partial class ItemTypeUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "Post");

            migrationBuilder.AddColumn<int>(
                name: "ItemTypeId",
                table: "Post",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_ItemTypeId",
                table: "Post",
                column: "ItemTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_ItemType_ItemTypeId",
                table: "Post",
                column: "ItemTypeId",
                principalTable: "ItemType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_ItemType_ItemTypeId",
                table: "Post");

            migrationBuilder.DropTable(
                name: "ItemType");

            migrationBuilder.DropIndex(
                name: "IX_Post_ItemTypeId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "ItemTypeId",
                table: "Post");

            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "Post",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
