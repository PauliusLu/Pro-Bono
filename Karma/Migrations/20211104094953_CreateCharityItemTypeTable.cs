using Microsoft.EntityFrameworkCore.Migrations;

namespace Karma.Migrations
{
    public partial class CreateCharityItemTypeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharityItemType",
                columns: table => new
                {
                    CharityId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityItemType", x => new { x.CharityId, x.ItemTypeId });
                    table.ForeignKey(
                        name: "FK_CharityItemType_Charity_Id",
                        column: x => x.CharityId,
                        principalTable: "Charity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharityItemType");
        }
    }
}
