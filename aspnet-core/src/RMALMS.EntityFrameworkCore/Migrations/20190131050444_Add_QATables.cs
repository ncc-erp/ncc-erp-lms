using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Add_QATables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QAQuestions",
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
                    Title = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    CourseInstanceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QAQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QAQuestions_CourseInstances_CourseInstanceId",
                        column: x => x.CourseInstanceId,
                        principalTable: "CourseInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QAAnswers",
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
                    Content = table.Column<string>(nullable: true),
                    QuestionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QAAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QAAnswers_QAQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QAQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFollowQAs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    QuestionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollowQAs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFollowQAs_QAQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QAQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFollowQAs_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QAAnswers_QuestionId",
                table: "QAAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QAQuestions_CourseInstanceId",
                table: "QAQuestions",
                column: "CourseInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowQAs_QuestionId",
                table: "UserFollowQAs",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowQAs_UserId",
                table: "UserFollowQAs",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QAAnswers");

            migrationBuilder.DropTable(
                name: "UserFollowQAs");

            migrationBuilder.DropTable(
                name: "QAQuestions");
        }
    }
}
