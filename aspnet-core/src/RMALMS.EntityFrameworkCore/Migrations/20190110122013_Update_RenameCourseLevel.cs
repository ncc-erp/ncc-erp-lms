using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_RenameCourseLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseStatuses_StatusId",
                table: "Courses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseStatuses",
                table: "CourseStatuses");

            migrationBuilder.RenameTable(
                name: "CourseStatuses",
                newName: "CourseLevels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseLevels",
                table: "CourseLevels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseLevels_StatusId",
                table: "Courses",
                column: "StatusId",
                principalTable: "CourseLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseLevels_StatusId",
                table: "Courses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseLevels",
                table: "CourseLevels");

            migrationBuilder.RenameTable(
                name: "CourseLevels",
                newName: "CourseStatuses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseStatuses",
                table: "CourseStatuses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseStatuses_StatusId",
                table: "Courses",
                column: "StatusId",
                principalTable: "CourseStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
