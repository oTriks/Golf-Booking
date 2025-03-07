using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golf_Booking.Migrations
{
    /// <inheritdoc />
    public partial class AddClubTypeProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "GolfClubs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "GolfClubs");
        }
    }
}
