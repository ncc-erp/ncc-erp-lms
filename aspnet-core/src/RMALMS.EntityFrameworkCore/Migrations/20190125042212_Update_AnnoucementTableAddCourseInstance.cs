using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_AnnoucementTableAddCourseInstance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CourseInstanceId",
                table: "Annoucements",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Annoucements_CourseInstanceId",
                table: "Annoucements",
                column: "CourseInstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Annoucements_CourseInstances_CourseInstanceId",
                table: "Annoucements",
                column: "CourseInstanceId",
                principalTable: "CourseInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Annoucements_CourseInstances_CourseInstanceId",
                table: "Annoucements");

            migrationBuilder.DropIndex(
                name: "IX_Annoucements_CourseInstanceId",
                table: "Annoucements");

            migrationBuilder.DropColumn(
                name: "CourseInstanceId",
                table: "Annoucements");
        }
    }
}
