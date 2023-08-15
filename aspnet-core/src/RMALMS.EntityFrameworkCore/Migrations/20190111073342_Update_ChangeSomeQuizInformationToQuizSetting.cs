using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_ChangeSomeQuizInformationToQuizSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowAttempts",
                table: "QuizSettings");

            migrationBuilder.DropColumn(
                name: "IsShuffleAnswer",
                table: "QuizSettings");

            migrationBuilder.DropColumn(
                name: "LookQuestionAfterAnswer",
                table: "QuizSettings");

            migrationBuilder.DropColumn(
                name: "ResponseType",
                table: "QuizSettings");

            migrationBuilder.DropColumn(
                name: "ScoreKeepType",
                table: "QuizSettings");

            migrationBuilder.DropColumn(
                name: "ShowOneQuestionAtATime",
                table: "QuizSettings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "QuizSettings");

            migrationBuilder.DropColumn(
                name: "TimeLimit",
                table: "QuizSettings");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "QuizSettings");

            migrationBuilder.AddColumn<int>(
                name: "AllowAttempts",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShuffleAnswer",
                table: "Quizzes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LookQuestionAfterAnswer",
                table: "Quizzes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "ResponseType",
                table: "Quizzes",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ScoreKeepType",
                table: "Quizzes",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOneQuestionAtATime",
                table: "Quizzes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "Quizzes",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "TimeLimit",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "Quizzes",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowAttempts",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "IsShuffleAnswer",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "LookQuestionAfterAnswer",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "ResponseType",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "ScoreKeepType",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "ShowOneQuestionAtATime",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "TimeLimit",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Quizzes");

            migrationBuilder.AddColumn<int>(
                name: "AllowAttempts",
                table: "QuizSettings",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShuffleAnswer",
                table: "QuizSettings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LookQuestionAfterAnswer",
                table: "QuizSettings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "ResponseType",
                table: "QuizSettings",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ScoreKeepType",
                table: "QuizSettings",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOneQuestionAtATime",
                table: "QuizSettings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "QuizSettings",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "TimeLimit",
                table: "QuizSettings",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "QuizSettings",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
