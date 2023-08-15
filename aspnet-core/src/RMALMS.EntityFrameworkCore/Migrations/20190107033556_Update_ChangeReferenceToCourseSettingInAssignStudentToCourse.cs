using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_ChangeReferenceToCourseSettingInAssignStudentToCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroups_Courses_CourseId",
                table: "CourseGroups");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "CourseGroups",
                newName: "CourseSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseGroups_CourseId",
                table: "CourseGroups",
                newName: "IX_CourseGroups_CourseSettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroups_CourseSettings_CourseSettingId",
                table: "CourseGroups",
                column: "CourseSettingId",
                principalTable: "CourseSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroups_CourseSettings_CourseSettingId",
                table: "CourseGroups");

            migrationBuilder.RenameColumn(
                name: "CourseSettingId",
                table: "CourseGroups",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseGroups_CourseSettingId",
                table: "CourseGroups",
                newName: "IX_CourseGroups_CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroups_Courses_CourseId",
                table: "CourseGroups",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
