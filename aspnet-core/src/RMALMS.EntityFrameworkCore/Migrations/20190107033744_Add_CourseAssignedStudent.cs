using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Add_CourseAssignedStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseAssignedStudents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    StudentId = table.Column<long>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    CourseSettingId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseAssignedStudents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseAssignedStudents_CourseSettings_CourseSettingId",
                        column: x => x.CourseSettingId,
                        principalTable: "CourseSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseAssignedStudents_AbpUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseAssignedStudents_CourseSettingId",
                table: "CourseAssignedStudents",
                column: "CourseSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAssignedStudents_StudentId",
                table: "CourseAssignedStudents",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseAssignedStudents");
        }
    }
}
