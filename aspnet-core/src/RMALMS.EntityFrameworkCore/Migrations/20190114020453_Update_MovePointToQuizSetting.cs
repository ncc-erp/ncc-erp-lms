using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_MovePointToQuizSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourseGroups_AbpUsers_StudentId",
                table: "StudentCourseGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLinks_AbpUsers_UserId",
                table: "UserLinks");

            migrationBuilder.DropIndex(
                name: "IX_UserLinks_UserId",
                table: "UserLinks");

            migrationBuilder.DropIndex(
                name: "IX_StudentCourseGroups_StudentId",
                table: "StudentCourseGroups");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserLinks");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "StudentCourseGroups");

            migrationBuilder.DropColumn(
                name: "Point",
                table: "QuizSettings");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedStudentId",
                table: "StudentCourseGroups",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<float>(
                name: "Point",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QuizId",
                table: "QuizSettings",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "AssignedSource",
                table: "CourseAssignedStudents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "AssingmentId",
                table: "AssignmentSettings",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseGroups_AssignedStudentId",
                table: "StudentCourseGroups",
                column: "AssignedStudentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSettings_QuizId",
                table: "QuizSettings",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSettings_AssingmentId",
                table: "AssignmentSettings",
                column: "AssingmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSettings_Assignments_AssingmentId",
                table: "AssignmentSettings",
                column: "AssingmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizSettings_Quizzes_QuizId",
                table: "QuizSettings",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourseGroups_CourseAssignedStudents_AssignedStudentId",
                table: "StudentCourseGroups",
                column: "AssignedStudentId",
                principalTable: "CourseAssignedStudents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentSettings_Assignments_AssingmentId",
                table: "AssignmentSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizSettings_Quizzes_QuizId",
                table: "QuizSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourseGroups_CourseAssignedStudents_AssignedStudentId",
                table: "StudentCourseGroups");

            migrationBuilder.DropIndex(
                name: "IX_StudentCourseGroups_AssignedStudentId",
                table: "StudentCourseGroups");

            migrationBuilder.DropIndex(
                name: "IX_QuizSettings_QuizId",
                table: "QuizSettings");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentSettings_AssingmentId",
                table: "AssignmentSettings");

            migrationBuilder.DropColumn(
                name: "AssignedStudentId",
                table: "StudentCourseGroups");

            migrationBuilder.DropColumn(
                name: "Point",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "QuizSettings");

            migrationBuilder.DropColumn(
                name: "AssignedSource",
                table: "CourseAssignedStudents");

            migrationBuilder.DropColumn(
                name: "AssingmentId",
                table: "AssignmentSettings");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "UserLinks",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StudentId",
                table: "StudentCourseGroups",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<float>(
                name: "Point",
                table: "QuizSettings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLinks_UserId",
                table: "UserLinks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseGroups_StudentId",
                table: "StudentCourseGroups",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourseGroups_AbpUsers_StudentId",
                table: "StudentCourseGroups",
                column: "StudentId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLinks_AbpUsers_UserId",
                table: "UserLinks",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
