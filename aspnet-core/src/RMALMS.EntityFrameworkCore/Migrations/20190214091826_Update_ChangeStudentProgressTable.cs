using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_ChangeStudentProgressTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentProgresses_Modules_ModudleId",
                table: "StudentProgresses");

            migrationBuilder.RenameColumn(
                name: "ModudleId",
                table: "StudentProgresses",
                newName: "PageId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentProgresses_ModudleId",
                table: "StudentProgresses",
                newName: "IX_StudentProgresses_PageId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProgresses_Pages_PageId",
                table: "StudentProgresses",
                column: "PageId",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentProgresses_Pages_PageId",
                table: "StudentProgresses");

            migrationBuilder.RenameColumn(
                name: "PageId",
                table: "StudentProgresses",
                newName: "ModudleId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentProgresses_PageId",
                table: "StudentProgresses",
                newName: "IX_StudentProgresses_ModudleId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProgresses_Modules_ModudleId",
                table: "StudentProgresses",
                column: "ModudleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
