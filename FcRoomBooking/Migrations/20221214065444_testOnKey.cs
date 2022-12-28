using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcRoomBooking.Migrations
{
    public partial class testOnKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_AspNetUsers_UserId",
                table: "Participants");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Participants_AspNetUsers_UserId",
                table: "Participants",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
