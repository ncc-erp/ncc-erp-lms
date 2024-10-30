using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Add_AddCourseCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Modules_ModuleId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_ModuleId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "StudentProgresses");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "Questions");

            migrationBuilder.AddColumn<byte>(
                name: "Progress",
                table: "StudentProgresses",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<bool>(
                name: "EnableCourseGradingScheme",
                table: "CourseSettings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "CourseSettings",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "Courses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RealtedImage",
                table: "Courses",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "State",
                table: "Courses",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "Syllabus",
                table: "Courses",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "CourseAssignedStudents",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TimeZoneId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Annoucements",
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
                    Title = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Annoucements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
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
                    Title = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    CourseSettingId = table.Column<Guid>(nullable: false),
                    Point = table.Column<float>(nullable: false),
                    DisplayGrade = table.Column<byte>(nullable: false),
                    SubmissionType = table.Column<byte>(nullable: false),
                    StartTimeUtc = table.Column<DateTime>(nullable: true),
                    EndTimeUtc = table.Column<DateTime>(nullable: true),
                    NumberOfDueDays = table.Column<int>(nullable: true),
                    Status = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_CourseSettings_CourseSettingId",
                        column: x => x.CourseSettingId,
                        principalTable: "CourseSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
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
                    Title = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    IsShuffleAnswer = table.Column<bool>(nullable: false),
                    TimeLimit = table.Column<int>(nullable: true),
                    AllowAttempts = table.Column<int>(nullable: true),
                    ScoreKeepType = table.Column<byte>(nullable: false),
                    ShowOneQuestionAtATime = table.Column<bool>(nullable: false),
                    LookQuestionAfterAnswer = table.Column<bool>(nullable: false),
                    ResponseType = table.Column<byte>(nullable: false),
                    NoOfDueDays = table.Column<int>(nullable: true),
                    StartTimeUtc = table.Column<DateTime>(nullable: true),
                    EndTimeUtc = table.Column<DateTime>(nullable: true),
                    Status = table.Column<byte>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    Point = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserCertifications",
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
                    StudentId = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    CourseId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Certification = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCertifications_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCertifications_AbpUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTimeZones",
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
                    TimeZoneId = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTimeZones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupAssingedAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    GroupId = table.Column<Guid>(nullable: false),
                    CourseAssignmentId = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAssingedAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupAssingedAssignments_Assignments_CourseAssignmentId",
                        column: x => x.CourseAssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupAssingedAssignments_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAssingedAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    StudentId = table.Column<long>(nullable: false),
                    CourseAssignmentId = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAssingedAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAssingedAssignments_Assignments_CourseAssignmentId",
                        column: x => x.CourseAssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAssingedAssignments_AbpUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupAssingedQuizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    GroupId = table.Column<Guid>(nullable: false),
                    QuizId = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAssingedQuizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupAssingedQuizzes_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupAssingedQuizzes_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionQuizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    QuestionId = table.Column<Guid>(nullable: false),
                    QuizId = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionQuizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionQuizzes_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionQuizzes_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAssingedQuizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    StudentId = table.Column<long>(nullable: false),
                    QuizId = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAssingedQuizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAssingedQuizzes_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAssingedQuizzes_AbpUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_LanguageId",
                table: "Courses",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAssignedStudents_CourseId",
                table: "CourseAssignedStudents",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_LanguageId",
                table: "AbpUsers",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_TimeZoneId",
                table: "AbpUsers",
                column: "TimeZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CourseSettingId",
                table: "Assignments",
                column: "CourseSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAssingedAssignments_CourseAssignmentId",
                table: "GroupAssingedAssignments",
                column: "CourseAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAssingedAssignments_GroupId",
                table: "GroupAssingedAssignments",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAssingedQuizzes_GroupId",
                table: "GroupAssingedQuizzes",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAssingedQuizzes_QuizId",
                table: "GroupAssingedQuizzes",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionQuizzes_QuestionId",
                table: "QuestionQuizzes",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionQuizzes_QuizId",
                table: "QuestionQuizzes",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssingedAssignments_CourseAssignmentId",
                table: "StudentAssingedAssignments",
                column: "CourseAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssingedAssignments_StudentId",
                table: "StudentAssingedAssignments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssingedQuizzes_QuizId",
                table: "StudentAssingedQuizzes",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssingedQuizzes_StudentId",
                table: "StudentAssingedQuizzes",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCertifications_CourseId",
                table: "UserCertifications",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCertifications_StudentId",
                table: "UserCertifications",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_AbpLanguages_LanguageId",
                table: "AbpUsers",
                column: "LanguageId",
                principalTable: "AbpLanguages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_UserTimeZones_TimeZoneId",
                table: "AbpUsers",
                column: "TimeZoneId",
                principalTable: "UserTimeZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAssignedStudents_Courses_CourseId",
                table: "CourseAssignedStudents",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AbpLanguages_LanguageId",
                table: "Courses",
                column: "LanguageId",
                principalTable: "AbpLanguages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_AbpLanguages_LanguageId",
                table: "AbpUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_UserTimeZones_TimeZoneId",
                table: "AbpUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseAssignedStudents_Courses_CourseId",
                table: "CourseAssignedStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AbpLanguages_LanguageId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "Annoucements");

            migrationBuilder.DropTable(
                name: "GroupAssingedAssignments");

            migrationBuilder.DropTable(
                name: "GroupAssingedQuizzes");

            migrationBuilder.DropTable(
                name: "QuestionQuizzes");

            migrationBuilder.DropTable(
                name: "StudentAssingedAssignments");

            migrationBuilder.DropTable(
                name: "StudentAssingedQuizzes");

            migrationBuilder.DropTable(
                name: "UserCertifications");

            migrationBuilder.DropTable(
                name: "UserLinks");

            migrationBuilder.DropTable(
                name: "UserTimeZones");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Courses_LanguageId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_CourseAssignedStudents_CourseId",
                table: "CourseAssignedStudents");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_LanguageId",
                table: "AbpUsers");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_TimeZoneId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "StudentProgresses");

            migrationBuilder.DropColumn(
                name: "EnableCourseGradingScheme",
                table: "CourseSettings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CourseSettings");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "RealtedImage",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Syllabus",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "CourseAssignedStudents");

            migrationBuilder.DropColumn(
                name: "Biography",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "AbpUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "Status",
                table: "StudentProgresses",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Group",
                table: "Questions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ModuleId",
                table: "Questions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ModuleId",
                table: "Questions",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Modules_ModuleId",
                table: "Questions",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
