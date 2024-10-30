using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Update_User_UserPersonalCertificationViewByPublic_UserPersonalCertificationViewByPublic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UserPersonalAchievementViewByPublic",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UserPersonalCertificationViewByPublic",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPersonalAchievementViewByPublic",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "UserPersonalCertificationViewByPublic",
                table: "AbpUsers");
        }
    }
}
