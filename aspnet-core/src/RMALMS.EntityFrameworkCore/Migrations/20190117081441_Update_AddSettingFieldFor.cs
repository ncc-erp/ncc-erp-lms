using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_AddSettingFieldFor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UserPersonalInfoViewByPublic",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UserPersonalLinksViewByPublic",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPersonalInfoViewByPublic",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "UserPersonalLinksViewByPublic",
                table: "AbpUsers");
        }
    }
}
