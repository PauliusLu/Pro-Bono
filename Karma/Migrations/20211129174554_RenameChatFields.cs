using Microsoft.EntityFrameworkCore.Migrations;

namespace Karma.Migrations
{
    public partial class RenameChatFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_AspNetUsers_CreatorIdId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_Chat_AspNetUsers_PostUserIdId",
                table: "Chat");

            migrationBuilder.RenameColumn(
                name: "PostUserIdId",
                table: "Chat",
                newName: "PostUserId");

            migrationBuilder.RenameColumn(
                name: "CreatorIdId",
                table: "Chat",
                newName: "CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Chat_PostUserIdId",
                table: "Chat",
                newName: "IX_Chat_PostUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Chat_CreatorIdId",
                table: "Chat",
                newName: "IX_Chat_CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_AspNetUsers_CreatorId",
                table: "Chat",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_AspNetUsers_PostUserId",
                table: "Chat",
                column: "PostUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_AspNetUsers_CreatorId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_Chat_AspNetUsers_PostUserId",
                table: "Chat");

            migrationBuilder.RenameColumn(
                name: "PostUserId",
                table: "Chat",
                newName: "PostUserIdId");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Chat",
                newName: "CreatorIdId");

            migrationBuilder.RenameIndex(
                name: "IX_Chat_PostUserId",
                table: "Chat",
                newName: "IX_Chat_PostUserIdId");

            migrationBuilder.RenameIndex(
                name: "IX_Chat_CreatorId",
                table: "Chat",
                newName: "IX_Chat_CreatorIdId");

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
        }
    }
}
