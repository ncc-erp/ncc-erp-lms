using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_UpdateAnswerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Answers",
                newName: "RAnswer");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Answers",
                newName: "LAnswer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RAnswer",
                table: "Answers",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "LAnswer",
                table: "Answers",
                newName: "Description");
        }
    }
}
