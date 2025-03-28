using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GolfBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddGolfCourseTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "GolfCourses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "GolfCourses");
        }
    }
}
