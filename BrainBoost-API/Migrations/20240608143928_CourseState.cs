using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainBoost_API.Migrations
{
    /// <inheritdoc />
    public partial class CourseState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CertificateState",
                table: "StudentEnrolledCourses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "QuizState",
                table: "StudentEnrolledCourses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "videoStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoId = table.Column<int>(type: "int", nullable: false),
                    StudentEnrolledCourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_videoStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_videoStates_StudentEnrolledCourses_StudentEnrolledCourseId",
                        column: x => x.StudentEnrolledCourseId,
                        principalTable: "StudentEnrolledCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_videoStates_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_videoStates_StudentEnrolledCourseId",
                table: "videoStates",
                column: "StudentEnrolledCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_videoStates_VideoId",
                table: "videoStates",
                column: "VideoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "videoStates");

            migrationBuilder.DropColumn(
                name: "CertificateState",
                table: "StudentEnrolledCourses");

            migrationBuilder.DropColumn(
                name: "QuizState",
                table: "StudentEnrolledCourses");
        }
    }
}
