using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Add_AddStudentAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "StudentAssignments");

            migrationBuilder.CreateTable(
                name: "StudentAssignmentFiles",
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
                    AssignmentSettingId = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    CourseGroupId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAssignmentFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAssignmentFiles_AssignmentSettings_AssignmentSettingId",
                        column: x => x.AssignmentSettingId,
                        principalTable: "AssignmentSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAssignmentFiles_CourseGroups_CourseGroupId",
                        column: x => x.CourseGroupId,
                        principalTable: "CourseGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignmentFiles_AssignmentSettingId",
                table: "StudentAssignmentFiles",
                column: "AssignmentSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignmentFiles_CourseGroupId",
                table: "StudentAssignmentFiles",
                column: "CourseGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAssignmentFiles");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "StudentAssignments",
                nullable: true);
        }
    }
}
