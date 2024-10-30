using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RMALMS.Migrations
{
    public partial class Add_RedesignDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_CourseSettings_CourseSettingId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseAssignedStudents_CourseSettings_CourseSettingId",
                table: "CourseAssignedStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroups_CourseSettings_CourseSettingId",
                table: "CourseGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroups_Groups_GroupId",
                table: "CourseGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupAssingedAssignments_Assignments_CourseAssignmentId",
                table: "GroupAssingedAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupAssingedQuizzes_Quizzes_QuizId",
                table: "GroupAssingedQuizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssingedAssignments_Assignments_CourseAssignmentId",
                table: "StudentAssingedAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssingedQuizzes_Quizzes_QuizId",
                table: "StudentAssingedQuizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProgresses_Courses_CourseId",
                table: "StudentProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProgresses_StudentEnrollments_EnrollmentId",
                table: "StudentProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_TestAttempts_StudentEnrollments_EnrollmentId",
                table: "TestAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_TestAttempts_Modules_ModuleId",
                table: "TestAttempts");

            migrationBuilder.DropTable(
                name: "ModuleSettings");

            migrationBuilder.DropTable(
                name: "StudentEnrollments");

            migrationBuilder.DropTable(
                name: "CourseSettings");

            migrationBuilder.DropIndex(
                name: "IX_TestAttempts_EnrollmentId",
                table: "TestAttempts");

            migrationBuilder.DropIndex(
                name: "IX_TestAttempts_ModuleId",
                table: "TestAttempts");

            migrationBuilder.DropIndex(
                name: "IX_StudentProgresses_EnrollmentId",
                table: "StudentProgresses");

            migrationBuilder.DropIndex(
                name: "IX_CourseGroups_GroupId",
                table: "CourseGroups");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_CourseSettingId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "EnrollmentId",
                table: "TestAttempts");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "TestAttempts");

            migrationBuilder.DropColumn(
                name: "EnrollmentId",
                table: "StudentProgresses");

            migrationBuilder.DropColumn(
                name: "AllowAttempts",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "EndTimeUtc",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "IsShuffleAnswer",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "LookQuestionAfterAnswer",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "NoOfDueDays",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Point",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "ResponseType",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "ScoreKeepType",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "ShowOneQuestionAtATime",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "StartTimeUtc",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "TimeLimit",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "CourseGroups");

            migrationBuilder.DropColumn(
                name: "CourseSettingId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "DisplayGrade",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "EndTimeUtc",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "NumberOfDueDays",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Point",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "StartTimeUtc",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "SubmissionType",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "StudentProgresses",
                newName: "CourseInstanceId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentProgresses_CourseId",
                table: "StudentProgresses",
                newName: "IX_StudentProgresses_CourseInstanceId");

            migrationBuilder.RenameColumn(
                name: "QuizId",
                table: "StudentAssingedQuizzes",
                newName: "QuizSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAssingedQuizzes_QuizId",
                table: "StudentAssingedQuizzes",
                newName: "IX_StudentAssingedQuizzes_QuizSettingId");

            migrationBuilder.RenameColumn(
                name: "CourseAssignmentId",
                table: "StudentAssingedAssignments",
                newName: "AssignmentSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAssingedAssignments_CourseAssignmentId",
                table: "StudentAssingedAssignments",
                newName: "IX_StudentAssingedAssignments_AssignmentSettingId");

            migrationBuilder.RenameColumn(
                name: "QuizId",
                table: "GroupAssingedQuizzes",
                newName: "QuizSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupAssingedQuizzes_QuizId",
                table: "GroupAssingedQuizzes",
                newName: "IX_GroupAssingedQuizzes_QuizSettingId");

            migrationBuilder.RenameColumn(
                name: "CourseAssignmentId",
                table: "GroupAssingedAssignments",
                newName: "AssignmentSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupAssingedAssignments_CourseAssignmentId",
                table: "GroupAssingedAssignments",
                newName: "IX_GroupAssingedAssignments_AssignmentSettingId");

            migrationBuilder.AddColumn<Guid>(
                name: "QuizSettingId",
                table: "TestAttempts",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "CourseGroups",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "CourseGroups",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CourseGroups",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "CourseGroups",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "CourseGroups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CourseGroups",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseInstances",
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
                    AllowSkip = table.Column<bool>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true),
                    PassingMark = table.Column<float>(nullable: false),
                    CourseId = table.Column<Guid>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TotalQuiz = table.Column<int>(nullable: false),
                    Version = table.Column<string>(nullable: true),
                    AllowFinalQuizRetry = table.Column<bool>(nullable: false),
                    NumberDayToStudy = table.Column<int>(nullable: true),
                    Status = table.Column<byte>(nullable: false),
                    EnableCourseGradingScheme = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseInstances_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GradeSchemes",
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
                    CourseId = table.Column<Guid>(nullable: false),
                    Status = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeSchemes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradeSchemes_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentCourseGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    StudentId = table.Column<long>(nullable: false),
                    CourseGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourseGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentCourseGroups_CourseGroups_CourseGroupId",
                        column: x => x.CourseGroupId,
                        principalTable: "CourseGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCourseGroups_AbpUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentSettings",
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
                    CourseInstanceId = table.Column<Guid>(nullable: false),
                    Point = table.Column<float>(nullable: true),
                    DisplayGrade = table.Column<byte>(nullable: false),
                    SubmissionType = table.Column<byte>(nullable: false),
                    StartTimeUtc = table.Column<DateTime>(nullable: true),
                    EndTimeUtc = table.Column<DateTime>(nullable: true),
                    NumberOfDueDays = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentSettings_CourseInstances_CourseInstanceId",
                        column: x => x.CourseInstanceId,
                        principalTable: "CourseInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupAssignedCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    CourseInstanceId = table.Column<Guid>(nullable: false),
                    GroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAssignedCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupAssignedCourses_CourseInstances_CourseInstanceId",
                        column: x => x.CourseInstanceId,
                        principalTable: "CourseInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupAssignedCourses_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizSettings",
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
                    Point = table.Column<float>(nullable: true),
                    CourseInstanceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizSettings_CourseInstances_CourseInstanceId",
                        column: x => x.CourseInstanceId,
                        principalTable: "CourseInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GradeSchemeElements",
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
                    GradeSchemeId = table.Column<Guid>(nullable: false),
                    LowRange = table.Column<float>(nullable: false),
                    HighRange = table.Column<float>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    LowCompareOperation = table.Column<int>(nullable: false),
                    HighCompareOpertion = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeSchemeElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradeSchemeElements_GradeSchemes_GradeSchemeId",
                        column: x => x.GradeSchemeId,
                        principalTable: "GradeSchemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    Point = table.Column<float>(nullable: true),
                    AssignmentSettingId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAssignments_AssignmentSettings_AssignmentSettingId",
                        column: x => x.AssignmentSettingId,
                        principalTable: "AssignmentSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempts_QuizSettingId",
                table: "TestAttempts",
                column: "QuizSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSettings_CourseInstanceId",
                table: "AssignmentSettings",
                column: "CourseInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseInstances_CourseId",
                table: "CourseInstances",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_GradeSchemeElements_GradeSchemeId",
                table: "GradeSchemeElements",
                column: "GradeSchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_GradeSchemes_CourseId",
                table: "GradeSchemes",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAssignedCourses_CourseInstanceId",
                table: "GroupAssignedCourses",
                column: "CourseInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAssignedCourses_GroupId",
                table: "GroupAssignedCourses",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSettings_CourseInstanceId",
                table: "QuizSettings",
                column: "CourseInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignments_AssignmentSettingId",
                table: "StudentAssignments",
                column: "AssignmentSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseGroups_CourseGroupId",
                table: "StudentCourseGroups",
                column: "CourseGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseGroups_StudentId",
                table: "StudentCourseGroups",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAssignedStudents_CourseInstances_CourseSettingId",
                table: "CourseAssignedStudents",
                column: "CourseSettingId",
                principalTable: "CourseInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroups_CourseInstances_CourseSettingId",
                table: "CourseGroups",
                column: "CourseSettingId",
                principalTable: "CourseInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupAssingedAssignments_AssignmentSettings_AssignmentSettingId",
                table: "GroupAssingedAssignments",
                column: "AssignmentSettingId",
                principalTable: "AssignmentSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupAssingedQuizzes_QuizSettings_QuizSettingId",
                table: "GroupAssingedQuizzes",
                column: "QuizSettingId",
                principalTable: "QuizSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssingedAssignments_AssignmentSettings_AssignmentSettingId",
                table: "StudentAssingedAssignments",
                column: "AssignmentSettingId",
                principalTable: "AssignmentSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssingedQuizzes_QuizSettings_QuizSettingId",
                table: "StudentAssingedQuizzes",
                column: "QuizSettingId",
                principalTable: "QuizSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProgresses_CourseInstances_CourseInstanceId",
                table: "StudentProgresses",
                column: "CourseInstanceId",
                principalTable: "CourseInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestAttempts_QuizSettings_QuizSettingId",
                table: "TestAttempts",
                column: "QuizSettingId",
                principalTable: "QuizSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAssignedStudents_CourseInstances_CourseSettingId",
                table: "CourseAssignedStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroups_CourseInstances_CourseSettingId",
                table: "CourseGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupAssingedAssignments_AssignmentSettings_AssignmentSettingId",
                table: "GroupAssingedAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupAssingedQuizzes_QuizSettings_QuizSettingId",
                table: "GroupAssingedQuizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssingedAssignments_AssignmentSettings_AssignmentSettingId",
                table: "StudentAssingedAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssingedQuizzes_QuizSettings_QuizSettingId",
                table: "StudentAssingedQuizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProgresses_CourseInstances_CourseInstanceId",
                table: "StudentProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_TestAttempts_QuizSettings_QuizSettingId",
                table: "TestAttempts");

            migrationBuilder.DropTable(
                name: "GradeSchemeElements");

            migrationBuilder.DropTable(
                name: "GroupAssignedCourses");

            migrationBuilder.DropTable(
                name: "QuizSettings");

            migrationBuilder.DropTable(
                name: "StudentAssignments");

            migrationBuilder.DropTable(
                name: "StudentCourseGroups");

            migrationBuilder.DropTable(
                name: "GradeSchemes");

            migrationBuilder.DropTable(
                name: "AssignmentSettings");

            migrationBuilder.DropTable(
                name: "CourseInstances");

            migrationBuilder.DropIndex(
                name: "IX_TestAttempts_QuizSettingId",
                table: "TestAttempts");

            migrationBuilder.DropColumn(
                name: "QuizSettingId",
                table: "TestAttempts");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "CourseGroups");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "CourseGroups");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CourseGroups");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "CourseGroups");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "CourseGroups");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CourseGroups");

            migrationBuilder.RenameColumn(
                name: "CourseInstanceId",
                table: "StudentProgresses",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentProgresses_CourseInstanceId",
                table: "StudentProgresses",
                newName: "IX_StudentProgresses_CourseId");

            migrationBuilder.RenameColumn(
                name: "QuizSettingId",
                table: "StudentAssingedQuizzes",
                newName: "QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAssingedQuizzes_QuizSettingId",
                table: "StudentAssingedQuizzes",
                newName: "IX_StudentAssingedQuizzes_QuizId");

            migrationBuilder.RenameColumn(
                name: "AssignmentSettingId",
                table: "StudentAssingedAssignments",
                newName: "CourseAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAssingedAssignments_AssignmentSettingId",
                table: "StudentAssingedAssignments",
                newName: "IX_StudentAssingedAssignments_CourseAssignmentId");

            migrationBuilder.RenameColumn(
                name: "QuizSettingId",
                table: "GroupAssingedQuizzes",
                newName: "QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupAssingedQuizzes_QuizSettingId",
                table: "GroupAssingedQuizzes",
                newName: "IX_GroupAssingedQuizzes_QuizId");

            migrationBuilder.RenameColumn(
                name: "AssignmentSettingId",
                table: "GroupAssingedAssignments",
                newName: "CourseAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupAssingedAssignments_AssignmentSettingId",
                table: "GroupAssingedAssignments",
                newName: "IX_GroupAssingedAssignments_CourseAssignmentId");

            migrationBuilder.AddColumn<Guid>(
                name: "EnrollmentId",
                table: "TestAttempts",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModuleId",
                table: "TestAttempts",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EnrollmentId",
                table: "StudentProgresses",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AllowAttempts",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTimeUtc",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShuffleAnswer",
                table: "Quizzes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LookQuestionAfterAnswer",
                table: "Quizzes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NoOfDueDays",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Point",
                table: "Quizzes",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<byte>(
                name: "ResponseType",
                table: "Quizzes",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ScoreKeepType",
                table: "Quizzes",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOneQuestionAtATime",
                table: "Quizzes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTimeUtc",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "Quizzes",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "TimeLimit",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "Quizzes",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "CourseGroups",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CourseSettingId",
                table: "Assignments",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<byte>(
                name: "DisplayGrade",
                table: "Assignments",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTimeUtc",
                table: "Assignments",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDueDays",
                table: "Assignments",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Point",
                table: "Assignments",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTimeUtc",
                table: "Assignments",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SubmissionType",
                table: "Assignments",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "CourseSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AllowFinalQuizRetry = table.Column<bool>(nullable: false),
                    AllowSkip = table.Column<bool>(nullable: false),
                    CourseId = table.Column<Guid>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    EnableCourseGradingScheme = table.Column<bool>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    NumberDayToStudy = table.Column<int>(nullable: true),
                    PassingMark = table.Column<float>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: true),
                    Status = table.Column<byte>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    TotalQuiz = table.Column<int>(nullable: false),
                    Version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseSettings_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModuleSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CourseSettingId = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ModuleId = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    WeightMark = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleSettings_CourseSettings_CourseSettingId",
                        column: x => x.CourseSettingId,
                        principalTable: "CourseSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleSettings_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentEnrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CourseId = table.Column<Guid>(nullable: false),
                    CourseSettingId = table.Column<Guid>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    StudentId = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentEnrollments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentEnrollments_CourseSettings_CourseSettingId",
                        column: x => x.CourseSettingId,
                        principalTable: "CourseSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentEnrollments_AbpUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempts_EnrollmentId",
                table: "TestAttempts",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempts_ModuleId",
                table: "TestAttempts",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProgresses_EnrollmentId",
                table: "StudentProgresses",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseGroups_GroupId",
                table: "CourseGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CourseSettingId",
                table: "Assignments",
                column: "CourseSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSettings_CourseId",
                table: "CourseSettings",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleSettings_CourseSettingId",
                table: "ModuleSettings",
                column: "CourseSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleSettings_ModuleId",
                table: "ModuleSettings",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_CourseId",
                table: "StudentEnrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_CourseSettingId",
                table: "StudentEnrollments",
                column: "CourseSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_StudentId",
                table: "StudentEnrollments",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_CourseSettings_CourseSettingId",
                table: "Assignments",
                column: "CourseSettingId",
                principalTable: "CourseSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAssignedStudents_CourseSettings_CourseSettingId",
                table: "CourseAssignedStudents",
                column: "CourseSettingId",
                principalTable: "CourseSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroups_CourseSettings_CourseSettingId",
                table: "CourseGroups",
                column: "CourseSettingId",
                principalTable: "CourseSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroups_Groups_GroupId",
                table: "CourseGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupAssingedAssignments_Assignments_CourseAssignmentId",
                table: "GroupAssingedAssignments",
                column: "CourseAssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupAssingedQuizzes_Quizzes_QuizId",
                table: "GroupAssingedQuizzes",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssingedAssignments_Assignments_CourseAssignmentId",
                table: "StudentAssingedAssignments",
                column: "CourseAssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssingedQuizzes_Quizzes_QuizId",
                table: "StudentAssingedQuizzes",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProgresses_Courses_CourseId",
                table: "StudentProgresses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProgresses_StudentEnrollments_EnrollmentId",
                table: "StudentProgresses",
                column: "EnrollmentId",
                principalTable: "StudentEnrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestAttempts_StudentEnrollments_EnrollmentId",
                table: "TestAttempts",
                column: "EnrollmentId",
                principalTable: "StudentEnrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestAttempts_Modules_ModuleId",
                table: "TestAttempts",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
