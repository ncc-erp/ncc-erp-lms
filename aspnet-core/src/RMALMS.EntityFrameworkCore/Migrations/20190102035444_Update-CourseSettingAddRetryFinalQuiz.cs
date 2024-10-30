using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class UpdateCourseSettingAddRetryFinalQuiz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_TestAttempts_TestAttempId",
                table: "StudentAnswers");

            migrationBuilder.AlterColumn<Guid>(
                name: "TestAttempId",
                table: "StudentAnswers",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "ModuleId",
                table: "StudentAnswers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowFinalQuizRetry",
                table: "CourseSettings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_ModuleId",
                table: "StudentAnswers",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_Modules_ModuleId",
                table: "StudentAnswers",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_TestAttempts_TestAttempId",
                table: "StudentAnswers",
                column: "TestAttempId",
                principalTable: "TestAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_Modules_ModuleId",
                table: "StudentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_TestAttempts_TestAttempId",
                table: "StudentAnswers");

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_ModuleId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "AllowFinalQuizRetry",
                table: "CourseSettings");

            migrationBuilder.AlterColumn<Guid>(
                name: "TestAttempId",
                table: "StudentAnswers",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_TestAttempts_TestAttempId",
                table: "StudentAnswers",
                column: "TestAttempId",
                principalTable: "TestAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
