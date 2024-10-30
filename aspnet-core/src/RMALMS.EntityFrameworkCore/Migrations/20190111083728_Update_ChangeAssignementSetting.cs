using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_ChangeAssignementSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayGrade",
                table: "AssignmentSettings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AssignmentSettings");

            migrationBuilder.DropColumn(
                name: "SubmissionType",
                table: "AssignmentSettings");

            migrationBuilder.AddColumn<byte>(
                name: "DisplayGrade",
                table: "Assignments",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "SubmissionType",
                table: "Assignments",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayGrade",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "SubmissionType",
                table: "Assignments");

            migrationBuilder.AddColumn<byte>(
                name: "DisplayGrade",
                table: "AssignmentSettings",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AssignmentSettings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "SubmissionType",
                table: "AssignmentSettings",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
