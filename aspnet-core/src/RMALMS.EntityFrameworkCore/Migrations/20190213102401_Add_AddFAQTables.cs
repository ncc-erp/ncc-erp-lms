using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Add_AddFAQTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowQAs_QAQuestions_QuestionId",
                table: "UserFollowQAs");

            migrationBuilder.DropIndex(
                name: "IX_UserFollowQAs_QuestionId",
                table: "UserFollowQAs");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "UserFollowQAs",
                newName: "FollowId");

            migrationBuilder.AddColumn<string>(
                name: "FollowType",
                table: "UserFollowQAs",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ResponseParentId",
                table: "QAAnswers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FAQQuestions",
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
                    CourseId = table.Column<Guid>(nullable: false),
                    SequenceOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FAQQuestions_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FAQAnswers",
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
                    QuestionId = table.Column<Guid>(nullable: false),
                    SequenceOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FAQAnswers_FAQQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "FAQQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QAAnswers_ResponseParentId",
                table: "QAAnswers",
                column: "ResponseParentId");

            migrationBuilder.CreateIndex(
                name: "IX_FAQAnswers_QuestionId",
                table: "FAQAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_FAQQuestions_CourseId",
                table: "FAQQuestions",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_QAAnswers_QAAnswers_ResponseParentId",
                table: "QAAnswers",
                column: "ResponseParentId",
                principalTable: "QAAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QAAnswers_QAAnswers_ResponseParentId",
                table: "QAAnswers");

            migrationBuilder.DropTable(
                name: "FAQAnswers");

            migrationBuilder.DropTable(
                name: "FAQQuestions");

            migrationBuilder.DropIndex(
                name: "IX_QAAnswers_ResponseParentId",
                table: "QAAnswers");

            migrationBuilder.DropColumn(
                name: "FollowType",
                table: "UserFollowQAs");

            migrationBuilder.DropColumn(
                name: "ResponseParentId",
                table: "QAAnswers");

            migrationBuilder.RenameColumn(
                name: "FollowId",
                table: "UserFollowQAs",
                newName: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowQAs_QuestionId",
                table: "UserFollowQAs",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowQAs_QAQuestions_QuestionId",
                table: "UserFollowQAs",
                column: "QuestionId",
                principalTable: "QAQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
