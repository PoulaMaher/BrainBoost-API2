using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainBoost_API.Migrations
{
    /// <inheritdoc />
    public partial class removeprop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "whatToLearn",
                table: "Courses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "whatToLearn",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
