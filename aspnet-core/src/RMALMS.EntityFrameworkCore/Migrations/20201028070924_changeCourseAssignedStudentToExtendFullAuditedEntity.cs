using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class changeCourseAssignedStudentToExtendFullAuditedEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "CourseAssignedStudents",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "CourseAssignedStudents",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CourseAssignedStudents",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "CourseAssignedStudents",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "CourseAssignedStudents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "CourseAssignedStudents");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "CourseAssignedStudents");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CourseAssignedStudents");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "CourseAssignedStudents");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "CourseAssignedStudents");
        }
    }
}
