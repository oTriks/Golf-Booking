using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golf_Booking.Migrations
{
    /// <inheritdoc />
    public partial class AddGolfClubToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GolfClubId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GolfClubId",
                table: "Users",
                column: "GolfClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_GolfClubs_GolfClubId",
                table: "Users",
                column: "GolfClubId",
                principalTable: "GolfClubs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_GolfClubs_GolfClubId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_GolfClubId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GolfClubId",
                table: "Users");
        }
    }
}
