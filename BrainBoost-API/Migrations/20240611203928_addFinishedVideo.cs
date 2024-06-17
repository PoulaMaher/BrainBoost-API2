using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainBoost_API.Migrations
{
    /// <inheritdoc />
    public partial class addFinishedVideo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

         


            migrationBuilder.AddColumn<bool>(
                name: "hasFinishedallVideos",
                table: "StudentEnrolledCourses",
                type: "bit",
                nullable: false,
                defaultValue: false);

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.DropColumn(
                name: "hasFinishedallVideos",
                table: "StudentEnrolledCourses");

         
           
        }
    }
}
