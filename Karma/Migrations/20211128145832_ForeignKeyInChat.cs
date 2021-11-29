using Microsoft.EntityFrameworkCore.Migrations;

namespace Karma.Migrations
{
    public partial class ForeignKeyInChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "PostUserId",
                table: "Chat");

            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "Message",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorIdId",
                table: "Chat",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostUserIdId",
                table: "Chat",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderId",
                table: "Message",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_CreatorIdId",
                table: "Chat",
                column: "CreatorIdId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_PostUserIdId",
                table: "Chat",
                column: "PostUserIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_AspNetUsers_CreatorIdId",
                table: "Chat",
                column: "CreatorIdId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_AspNetUsers_PostUserIdId",
                table: "Chat",
                column: "PostUserIdId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_AspNetUsers_SenderId",
                table: "Message",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_AspNetUsers_CreatorIdId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_Chat_AspNetUsers_PostUserIdId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_AspNetUsers_SenderId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_SenderId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Chat_CreatorIdId",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Chat_PostUserIdId",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "CreatorIdId",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "PostUserIdId",
                table: "Chat");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Message",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Chat",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostUserId",
                table: "Chat",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
