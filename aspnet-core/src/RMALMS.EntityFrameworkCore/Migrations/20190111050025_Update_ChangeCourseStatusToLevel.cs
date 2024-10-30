using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_ChangeCourseStatusToLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseLevels_StatusId",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Courses",
                newName: "LevelId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_StatusId",
                table: "Courses",
                newName: "IX_Courses_LevelId");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "UserLinks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLinks_UserId",
                table: "UserLinks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseLevels_LevelId",
                table: "Courses",
                column: "LevelId",
                principalTable: "CourseLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLinks_AbpUsers_UserId",
                table: "UserLinks",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseLevels_LevelId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLinks_AbpUsers_UserId",
                table: "UserLinks");

            migrationBuilder.DropIndex(
                name: "IX_UserLinks_UserId",
                table: "UserLinks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserLinks");

            migrationBuilder.RenameColumn(
                name: "LevelId",
                table: "Courses",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_LevelId",
                table: "Courses",
                newName: "IX_Courses_StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseLevels_StatusId",
                table: "Courses",
                column: "StatusId",
                principalTable: "CourseLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
