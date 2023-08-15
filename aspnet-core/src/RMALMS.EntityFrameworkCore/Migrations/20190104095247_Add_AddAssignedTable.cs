using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Add_AddAssignedTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberDayToStudy",
                table: "CourseSettings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReleatedInformation",
                table: "Courses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberDayToStudy",
                table: "CourseSettings");

            migrationBuilder.DropColumn(
                name: "ReleatedInformation",
                table: "Courses");
        }
    }
}
