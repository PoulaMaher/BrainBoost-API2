using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainBoost_API.Migrations
{
    /// <inheritdoc />
    public partial class enhanceCourseModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Answers_TrueAnswerId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "QuestionQuiz");

            migrationBuilder.DropIndex(
                name: "IX_Questions_TrueAnswerId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "TrueAnswerId",
                table: "Questions");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "QuizQuesitons",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "quizId",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "Answers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_quizId",
                table: "Courses",
                column: "quizId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Quizzes_quizId",
                table: "Courses",
                column: "quizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Quizzes_quizId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_quizId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "quizId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "Answers");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "QuizQuesitons",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TrueAnswerId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "QuestionQuiz",
                columns: table => new
                {
                    QuestionsId = table.Column<int>(type: "int", nullable: false),
                    QuizzesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionQuiz", x => new { x.QuestionsId, x.QuizzesId });
                    table.ForeignKey(
                        name: "FK_QuestionQuiz_Questions_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionQuiz_Quizzes_QuizzesId",
                        column: x => x.QuizzesId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_TrueAnswerId",
                table: "Questions",
                column: "TrueAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionQuiz_QuizzesId",
                table: "QuestionQuiz",
                column: "QuizzesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Answers_TrueAnswerId",
                table: "Questions",
                column: "TrueAnswerId",
                principalTable: "Answers",
                principalColumn: "Id");
        }
    }
}
