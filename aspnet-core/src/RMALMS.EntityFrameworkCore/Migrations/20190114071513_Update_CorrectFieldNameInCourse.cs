using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_CorrectFieldNameInCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReleatedInformation",
                table: "Courses",
                newName: "RelatedInformation");

            migrationBuilder.RenameColumn(
                name: "RealtedImage",
                table: "Courses",
                newName: "RelatedImage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RelatedInformation",
                table: "Courses",
                newName: "ReleatedInformation");

            migrationBuilder.RenameColumn(
                name: "RelatedImage",
                table: "Courses",
                newName: "RealtedImage");
        }
    }
}
