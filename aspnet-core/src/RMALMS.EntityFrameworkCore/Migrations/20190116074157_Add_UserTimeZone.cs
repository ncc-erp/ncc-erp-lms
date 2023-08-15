using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Add_UserTimeZone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "UserTimeZones",
                newName: "StandardName");

            migrationBuilder.AddColumn<string>(
                name: "BaseUtcOffset",
                table: "UserTimeZones",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "UserTimeZones",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SupportsDaylightSavingTime",
                table: "UserTimeZones",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "AssignedSource",
                table: "CourseAssignedStudents",
                nullable: false,
                oldClrType: typeof(byte));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseUtcOffset",
                table: "UserTimeZones");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "UserTimeZones");

            migrationBuilder.DropColumn(
                name: "SupportsDaylightSavingTime",
                table: "UserTimeZones");

            migrationBuilder.RenameColumn(
                name: "StandardName",
                table: "UserTimeZones",
                newName: "Title");

            migrationBuilder.AlterColumn<byte>(
                name: "AssignedSource",
                table: "CourseAssignedStudents",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
