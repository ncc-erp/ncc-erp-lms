using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_AddTitleFieldForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroups_CourseInstances_CourseSettingId",
                table: "CourseGroups");

            migrationBuilder.RenameColumn(
                name: "CourseSettingId",
                table: "CourseGroups",
                newName: "CourseInstanceId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseGroups_CourseSettingId",
                table: "CourseGroups",
                newName: "IX_CourseGroups_CourseInstanceId");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroups_CourseInstances_CourseInstanceId",
                table: "CourseGroups",
                column: "CourseInstanceId",
                principalTable: "CourseInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroups_CourseInstances_CourseInstanceId",
                table: "CourseGroups");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "AbpUsers");

            migrationBuilder.RenameColumn(
                name: "CourseInstanceId",
                table: "CourseGroups",
                newName: "CourseSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseGroups_CourseInstanceId",
                table: "CourseGroups",
                newName: "IX_CourseGroups_CourseSettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroups_CourseInstances_CourseSettingId",
                table: "CourseGroups",
                column: "CourseSettingId",
                principalTable: "CourseInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
