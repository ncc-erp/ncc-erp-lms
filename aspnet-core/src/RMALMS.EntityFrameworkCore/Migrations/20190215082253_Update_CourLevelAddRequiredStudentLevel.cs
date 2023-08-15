using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_CourLevelAddRequiredStudentLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LowCompareOperation",
                table: "CourseLevels",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredStudentLevel",
                table: "CourseLevels",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LowCompareOperation",
                table: "CourseLevels");

            migrationBuilder.DropColumn(
                name: "RequiredStudentLevel",
                table: "CourseLevels");
        }
    }
}
