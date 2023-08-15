using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_AddCourseIdToAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Point",
                table: "AssignmentSettings");

            migrationBuilder.AlterColumn<byte>(
                name: "AssignedSource",
                table: "CourseAssignedStudents",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "Assignments",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<float>(
                name: "Point",
                table: "Assignments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CourseId",
                table: "Assignments",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Courses_CourseId",
                table: "Assignments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Courses_CourseId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_CourseId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Point",
                table: "Assignments");

            migrationBuilder.AlterColumn<int>(
                name: "AssignedSource",
                table: "CourseAssignedStudents",
                nullable: false,
                oldClrType: typeof(byte));

            migrationBuilder.AddColumn<float>(
                name: "Point",
                table: "AssignmentSettings",
                nullable: true);
        }
    }
}
