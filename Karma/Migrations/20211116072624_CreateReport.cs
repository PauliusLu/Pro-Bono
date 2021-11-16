using Microsoft.EntityFrameworkCore.Migrations;

namespace Karma.Migrations
{
    public partial class CreateReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PostId = table.Column<int>(type: "INTEGER", nullable: false),
                    PostOwnerId = table.Column<string>(type: "TEXT", nullable: false),
                    ReporterId = table.Column<string>(type: "TEXT", nullable: false),
                    ReportState = table.Column<int>(type: "INTEGER", nullable: false),
                    ReportMessage = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Report");
        }
    }
}
