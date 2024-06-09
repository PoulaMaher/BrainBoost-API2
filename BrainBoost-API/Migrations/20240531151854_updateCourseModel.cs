using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainBoost_API.Migrations
{
    /// <inheritdoc />
    public partial class updateCourseModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Certificates_CertificateId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CertificateId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CertificateId",
                table: "Courses");

            migrationBuilder.AddColumn<string>(
                name: "CertificateAppreciationParagraph",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateHeadline",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificateAppreciationParagraph",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CertificateHeadline",
                table: "Courses");

            migrationBuilder.AddColumn<int>(
                name: "CertificateId",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CertificateId",
                table: "Courses",
                column: "CertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Certificates_CertificateId",
                table: "Courses",
                column: "CertificateId",
                principalTable: "Certificates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
