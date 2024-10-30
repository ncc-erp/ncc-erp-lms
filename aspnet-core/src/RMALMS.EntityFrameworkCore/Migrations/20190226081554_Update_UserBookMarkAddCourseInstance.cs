using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_UserBookMarkAddCourseInstance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CourseInstanceId",
                table: "UserBookMarks",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserBookMarks_CourseInstanceId",
                table: "UserBookMarks",
                column: "CourseInstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBookMarks_CourseInstances_CourseInstanceId",
                table: "UserBookMarks",
                column: "CourseInstanceId",
                principalTable: "CourseInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBookMarks_CourseInstances_CourseInstanceId",
                table: "UserBookMarks");

            migrationBuilder.DropIndex(
                name: "IX_UserBookMarks_CourseInstanceId",
                table: "UserBookMarks");

            migrationBuilder.DropColumn(
                name: "CourseInstanceId",
                table: "UserBookMarks");
        }
    }
}
