using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_ChangeCourseIntanceReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCertifications_Courses_CourseId",
                table: "UserCertifications");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "UserCertifications",
                newName: "CourseInstanceId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCertifications_CourseId",
                table: "UserCertifications",
                newName: "IX_UserCertifications_CourseInstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCertifications_CourseInstances_CourseInstanceId",
                table: "UserCertifications",
                column: "CourseInstanceId",
                principalTable: "CourseInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCertifications_CourseInstances_CourseInstanceId",
                table: "UserCertifications");

            migrationBuilder.RenameColumn(
                name: "CourseInstanceId",
                table: "UserCertifications",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCertifications_CourseInstanceId",
                table: "UserCertifications",
                newName: "IX_UserCertifications_CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCertifications_Courses_CourseId",
                table: "UserCertifications",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
