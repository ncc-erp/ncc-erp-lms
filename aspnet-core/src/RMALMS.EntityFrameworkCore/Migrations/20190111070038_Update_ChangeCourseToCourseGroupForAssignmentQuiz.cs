using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_ChangeCourseToCourseGroupForAssignmentQuiz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAssignedStudents_Courses_CourseId",
                table: "CourseAssignedStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseAssignedStudents_CourseInstances_CourseSettingId",
                table: "CourseAssignedStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupAssingedAssignments_Groups_GroupId",
                table: "GroupAssingedAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupAssingedQuizzes_Groups_GroupId",
                table: "GroupAssingedQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_CourseAssignedStudents_CourseId",
                table: "CourseAssignedStudents");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "CourseAssignedStudents");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "GroupAssingedQuizzes",
                newName: "CourseGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupAssingedQuizzes_GroupId",
                table: "GroupAssingedQuizzes",
                newName: "IX_GroupAssingedQuizzes_CourseGroupId");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "GroupAssingedAssignments",
                newName: "CourseGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupAssingedAssignments_GroupId",
                table: "GroupAssingedAssignments",
                newName: "IX_GroupAssingedAssignments_CourseGroupId");

            migrationBuilder.RenameColumn(
                name: "CourseSettingId",
                table: "CourseAssignedStudents",
                newName: "CourseInstanceId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseAssignedStudents_CourseSettingId",
                table: "CourseAssignedStudents",
                newName: "IX_CourseAssignedStudents_CourseInstanceId");

            migrationBuilder.AddColumn<bool>(
                name: "RestrictStudentFromViewThisCourseAfterEndDate",
                table: "Courses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RestrictStudentsFromViewingThisCourseBeforeEndDate",
                table: "Courses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StudentCanOnlyParticipiateCourseBetweenTheseDate",
                table: "Courses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAssignedStudents_CourseInstances_CourseInstanceId",
                table: "CourseAssignedStudents",
                column: "CourseInstanceId",
                principalTable: "CourseInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupAssingedAssignments_CourseGroups_CourseGroupId",
                table: "GroupAssingedAssignments",
                column: "CourseGroupId",
                principalTable: "CourseGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupAssingedQuizzes_CourseGroups_CourseGroupId",
                table: "GroupAssingedQuizzes",
                column: "CourseGroupId",
                principalTable: "CourseGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAssignedStudents_CourseInstances_CourseInstanceId",
                table: "CourseAssignedStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupAssingedAssignments_CourseGroups_CourseGroupId",
                table: "GroupAssingedAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupAssingedQuizzes_CourseGroups_CourseGroupId",
                table: "GroupAssingedQuizzes");

            migrationBuilder.DropColumn(
                name: "RestrictStudentFromViewThisCourseAfterEndDate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "RestrictStudentsFromViewingThisCourseBeforeEndDate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "StudentCanOnlyParticipiateCourseBetweenTheseDate",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "CourseGroupId",
                table: "GroupAssingedQuizzes",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupAssingedQuizzes_CourseGroupId",
                table: "GroupAssingedQuizzes",
                newName: "IX_GroupAssingedQuizzes_GroupId");

            migrationBuilder.RenameColumn(
                name: "CourseGroupId",
                table: "GroupAssingedAssignments",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupAssingedAssignments_CourseGroupId",
                table: "GroupAssingedAssignments",
                newName: "IX_GroupAssingedAssignments_GroupId");

            migrationBuilder.RenameColumn(
                name: "CourseInstanceId",
                table: "CourseAssignedStudents",
                newName: "CourseSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseAssignedStudents_CourseInstanceId",
                table: "CourseAssignedStudents",
                newName: "IX_CourseAssignedStudents_CourseSettingId");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "CourseAssignedStudents",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CourseAssignedStudents_CourseId",
                table: "CourseAssignedStudents",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAssignedStudents_Courses_CourseId",
                table: "CourseAssignedStudents",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAssignedStudents_CourseInstances_CourseSettingId",
                table: "CourseAssignedStudents",
                column: "CourseSettingId",
                principalTable: "CourseInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupAssingedAssignments_Groups_GroupId",
                table: "GroupAssingedAssignments",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupAssingedQuizzes_Groups_GroupId",
                table: "GroupAssingedQuizzes",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
