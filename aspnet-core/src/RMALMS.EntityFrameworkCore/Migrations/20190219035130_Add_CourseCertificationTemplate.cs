using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Add_CourseCertificationTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LowCompareOperation",
                table: "UserStatuses",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredNumber",
                table: "UserStatuses",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseCertificationTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    CourseId = table.Column<Guid>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Background = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    CertificationType = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCertificationTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseCertificationTemplates_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificationTemplateGradeSchemes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    CourseCertificationTemplateId = table.Column<Guid>(nullable: false),
                    GradeSchemeElementId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificationTemplateGradeSchemes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificationTemplateGradeSchemes_CourseCertificationTemplates_CourseCertificationTemplateId",
                        column: x => x.CourseCertificationTemplateId,
                        principalTable: "CourseCertificationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificationTemplateGradeSchemes_GradeSchemeElements_GradeSchemeElementId",
                        column: x => x.GradeSchemeElementId,
                        principalTable: "GradeSchemeElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificationTemplateGradeSchemes_CourseCertificationTemplateId",
                table: "CertificationTemplateGradeSchemes",
                column: "CourseCertificationTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificationTemplateGradeSchemes_GradeSchemeElementId",
                table: "CertificationTemplateGradeSchemes",
                column: "GradeSchemeElementId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCertificationTemplates_CourseId",
                table: "CourseCertificationTemplates",
                column: "CourseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificationTemplateGradeSchemes");

            migrationBuilder.DropTable(
                name: "CourseCertificationTemplates");

            migrationBuilder.DropColumn(
                name: "LowCompareOperation",
                table: "UserStatuses");

            migrationBuilder.DropColumn(
                name: "RequiredNumber",
                table: "UserStatuses");
        }
    }
}
