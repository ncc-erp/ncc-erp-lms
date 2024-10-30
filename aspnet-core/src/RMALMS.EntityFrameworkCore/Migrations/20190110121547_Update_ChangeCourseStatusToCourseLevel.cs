using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_ChangeCourseStatusToCourseLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseInstances_Courses_CourseId",
                table: "CourseInstances");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "CourseInstances",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseInstances_Courses_CourseId",
                table: "CourseInstances",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseInstances_Courses_CourseId",
                table: "CourseInstances");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "CourseInstances",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_CourseInstances_Courses_CourseId",
                table: "CourseInstances",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
