using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GolfBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddGolfClubAndCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "GolfBookings",
                newName: "GolfCourseId");

            migrationBuilder.CreateTable(
                name: "GolfClubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GolfClubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GolfCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GolfClubId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GolfCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GolfCourses_GolfClubs_GolfClubId",
                        column: x => x.GolfClubId,
                        principalTable: "GolfClubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GolfBookings_GolfCourseId",
                table: "GolfBookings",
                column: "GolfCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_GolfCourses_GolfClubId",
                table: "GolfCourses",
                column: "GolfClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_GolfBookings_GolfCourses_GolfCourseId",
                table: "GolfBookings",
                column: "GolfCourseId",
                principalTable: "GolfCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GolfBookings_GolfCourses_GolfCourseId",
                table: "GolfBookings");

            migrationBuilder.DropTable(
                name: "GolfCourses");

            migrationBuilder.DropTable(
                name: "GolfClubs");

            migrationBuilder.DropIndex(
                name: "IX_GolfBookings_GolfCourseId",
                table: "GolfBookings");

            migrationBuilder.RenameColumn(
                name: "GolfCourseId",
                table: "GolfBookings",
                newName: "CourseId");
        }
    }
}
