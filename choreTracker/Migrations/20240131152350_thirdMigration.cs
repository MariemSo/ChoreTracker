using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace choreTracker.Migrations
{
    public partial class thirdMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_WorkerId",
                table: "Jobs");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_WorkerId",
                table: "Jobs",
                column: "WorkerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_WorkerId",
                table: "Jobs");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_WorkerId",
                table: "Jobs",
                column: "WorkerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
