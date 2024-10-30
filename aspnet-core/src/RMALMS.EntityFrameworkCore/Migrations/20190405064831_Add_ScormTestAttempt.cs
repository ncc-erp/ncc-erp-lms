using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Add_ScormTestAttempt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScormTestAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Status = table.Column<byte>(nullable: false),
                    Score = table.Column<float>(nullable: false),
                    MaxScore = table.Column<float>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CourseAssignedStudentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScormTestAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScormTestAttempts_CourseAssignedStudents_CourseAssignedStudentId",
                        column: x => x.CourseAssignedStudentId,
                        principalTable: "CourseAssignedStudents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScormTestAttempts_CourseAssignedStudentId",
                table: "ScormTestAttempts",
                column: "CourseAssignedStudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScormTestAttempts");
        }
    }
}
