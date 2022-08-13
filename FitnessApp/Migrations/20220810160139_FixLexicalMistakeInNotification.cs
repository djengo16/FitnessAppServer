using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessApp.Migrations
{
    public partial class FixLexicalMistakeInNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_RecepientId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "RecepientId",
                table: "Notifications",
                newName: "RecipientId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_RecepientId",
                table: "Notifications",
                newName: "IX_Notifications_RecipientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_RecipientId",
                table: "Notifications",
                column: "RecipientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_RecipientId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "RecipientId",
                table: "Notifications",
                newName: "RecepientId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_RecipientId",
                table: "Notifications",
                newName: "IX_Notifications_RecepientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_RecepientId",
                table: "Notifications",
                column: "RecepientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
