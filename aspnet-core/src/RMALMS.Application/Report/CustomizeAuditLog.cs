using Abp.Auditing;
using Abp.Domain.Repositories;
using Newtonsoft.Json;
using RMALMS.Courses;
using RMALMS.Entities;
using RMALMS.IoC;
using RMALMS.QAQuestions;
using RMALMS.Reports.Dto;
using RMALMS.StudentProgresses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RMALMS.Reports
{
    public class CustomizeAuditLog : AuditingStore
    {
        private readonly IWorkScope _ws;
        public CustomizeAuditLog(IRepository<AuditLog, long> auditLogRepository, IWorkScope workScope) : base(auditLogRepository)
        {
            this._ws = workScope;
        }

        public override Task SaveAsync(AuditInfo auditInfo)
        {
            Guid Id;
            PageCourseDto page;
            if (auditInfo.MethodName == "GetAll") return base.SaveAsync(auditInfo);

            #region User_Activities
            switch (auditInfo.MethodName)
            {
                // Course enrolment and re-enrolment + Instructor_Statistics_Course_enrolled_and_invited_student
                case nameof(UserAssignedToCourses.StudentEnrollCourse):
                    Id = getIdFromJson(auditInfo.Parameters, "courseInstanceId");
                    if (Id == Guid.Empty) break;
                    auditInfo.ClientName = ActionReport.User_Activities_Course_enrolment_and_re__enrolment.ToString()
                        + "; " + ActionReport.Instructor_Statistics_Course_enrolled_and_invited_student.ToString();
                    auditInfo.CustomData = string.Format("Enrolled the course \"{0}\"", GetCourseNameFromCourseInstanceId(Id));
                    return base.SaveAsync(auditInfo);
                case nameof(UserAssignedToCourses.StudentReEnrollCourse):
                    Id = getIdFromJson(auditInfo.Parameters, "courseInstanceId");
                    if (Id == Guid.Empty) break;
                    auditInfo.ClientName = ActionReport.User_Activities_Course_enrolment_and_re__enrolment.ToString()
                        + "; " + ActionReport.Instructor_Statistics_Course_enrolled_and_invited_student.ToString();
                    auditInfo.CustomData = string.Format("Re-enrolment the course \"{0}\"", GetCourseNameFromCourseInstanceId(Id));
                    return base.SaveAsync(auditInfo);

                // Start and Resume the course
                case nameof(StudentProgressAppService.GetStudentProgressesByCourseInstanceId):
                    Id = getIdFromJson(auditInfo.Parameters, "courseInstanceId");
                    if (Id == Guid.Empty) break;
                    var courseName = GetCourseNameFromCourseInstanceId(Id);
                    auditInfo.ClientName = ActionReport.User_Activities_Start_and_Resume_the_course.ToString();

                    auditInfo.CustomData = string.Format("Start or Resume the course \"{0}\"", courseName);
                    return base.SaveAsync(auditInfo);

                // Course page viewed
                case nameof(Pages.PageAppService.GetForStudent):
                    Id = getIdFromJson(auditInfo.Parameters, "id");
                    if (Id == Guid.Empty) break;
                    page = GetPageAndCourseNameFromPageId(Id);
                    auditInfo.ClientName = ActionReport.User_Activities_Course_page_viewed.ToString();
                    auditInfo.CustomData = string.Format("Viewed {0} on course \"{1}\"", page.PageName, page.CourseName);
                    return base.SaveAsync(auditInfo);

                // Add or remove bookmark on course page
                case nameof(Pages.PageAppService.BookmarkPage):
                    Id = getIdFromJson(auditInfo.Parameters, "pageId");
                    if (Id == Guid.Empty) break;
                    page = GetPageAndCourseNameFromPageId(Id);
                    auditInfo.ClientName = ActionReport.User_Activities_Add_or_remove_bookmark_on_course_page.ToString();
                    auditInfo.CustomData = string.Format("Bookmarked {0} on course \"{1}\"", page.PageName, page.CourseName);
                    return base.SaveAsync(auditInfo);
                case nameof(Pages.PageAppService.UnBookmarkPage):
                    Id = getIdFromJson(auditInfo.Parameters, "id");
                    if (Id == Guid.Empty) break;
                    page = GetPageAndCourseNameFromPageId(Id);
                    auditInfo.ClientName = ActionReport.User_Activities_Add_or_remove_bookmark_on_course_page.ToString();
                    auditInfo.CustomData = string.Format("Remove Bookmarked {0} on course \"{1}\"", page.PageName, page.CourseName);
                    return base.SaveAsync(auditInfo);

                // Viewed on Q&A
                case nameof(QAQuestionAppService.GetQAQuestionAnswer):
                    auditInfo.ClientName = ActionReport.User_Activities_Viewed_on_QA.ToString();
                    auditInfo.CustomData = string.Format("Viewed Q&A");
                    return base.SaveAsync(auditInfo);

                // Post and reply on Q & A
                case nameof(QAQuestionAppService.CreateQAQuestion):
                    auditInfo.ClientName = ActionReport.User_Activities_Post_and_reply_on_QA.ToString();
                    auditInfo.CustomData = string.Format("Create Q&A question");
                    return base.SaveAsync(auditInfo);
                case nameof(QAQuestionAppService.CreateQAAnswer):
                    string studentName = getNameFromJson(auditInfo.Parameters, "replyUserName");
                    auditInfo.ClientName = ActionReport.User_Activities_Post_and_reply_on_QA.ToString();
                    auditInfo.CustomData = string.Format("Replied Student {0}", studentName);
                    return base.SaveAsync(auditInfo);

                // Viewed on course announcement 
                case nameof(Annoucements.AnnoucementAppService.GetAnnoucementForStudentByCourseInstanceIdPagging):
                    auditInfo.ClientName = ActionReport.User_Activities_Viewed_on_course_announcement.ToString();
                    auditInfo.CustomData = string.Format("Viewed announcement");
                    return base.SaveAsync(auditInfo);
            }


            switch (auditInfo.ServiceName)
            {

                // Quiz attempted
                case "RMALMS.Quizzes.QuizAppService":
                    {
                        auditInfo.ClientName = ActionReport.User_Activities_Quiz_attempted.ToString();
                        auditInfo.CustomData = string.Format("Quiz: {0}", auditInfo.MethodName);
                        return base.SaveAsync(auditInfo);
                    }

                // Assignment attempted 
                case "RMALMS.Assignments.AssignmentAppService":
                    {
                        auditInfo.ClientName = ActionReport.User_Activities_Assignment_attempted.ToString();
                        auditInfo.CustomData = string.Format("Assignment: {0}", auditInfo.MethodName);
                        return base.SaveAsync(auditInfo);
                    }

                // Admin Settings
                case "RMALMS.FeatureOptions.FeatureOptionAppService":
                    {
                        auditInfo.ClientName = ActionReport.User_Activities_Admin_Settings.ToString();
                        auditInfo.CustomData = string.Format("Admin Settings: {0}", auditInfo.MethodName);
                        return base.SaveAsync(auditInfo);
                    }

                // Report Downloaded
                case "RMALMS.Reports.ReportAppService":
                    {
                        if (auditInfo.MethodName == nameof(ReportAppService.CreateExportOfReportAuditLog))
                        {
                            auditInfo.ClientName = ActionReport.User_Activities_Report_Downloaded.ToString()
                                + "; " + ActionReport.Instructor_Statistics_Report_download.ToString();
                            auditInfo.CustomData = getNameFromJson(auditInfo.Parameters, "actionName");
                            return base.SaveAsync(auditInfo);
                        }
                        //else
                        //{
                        //    auditInfo.ClientName = ActionReport.User_Activities_Report_Downloaded.ToString();
                        //    auditInfo.CustomData = string.Format("Admin Reports: {0}", auditInfo.MethodName);
                        //    return base.SaveAsync(auditInfo);
                        //}
                        break;
                    }
            }

            #endregion User_Activities

            #region Student_Statistics

            //Student_Statistics_Course_Completion = 21,
            //Student_Statistics_Course_Final_Exam_Result = 22,
            //Student_Statistics_Course_Quiz_Completion = 24,
            if (auditInfo.ServiceName == "RMALMS.TestAttempts.TestAttemptAppService" && auditInfo.MethodName == "ProcessScore")
            {
                string type = getNameFromJson(auditInfo.Parameters, "type");
                if (type == "3")
                {
                    auditInfo.CustomData = string.Format("Completed the Final Quiz");
                }
                else
                {
                    auditInfo.CustomData = string.Format("Submitted Quiz");
                }
                auditInfo.ClientName = ActionReport.Student_Statistics_Course_Quiz_Completion.ToString();
                return base.SaveAsync(auditInfo);
            }


            //if (auditInfo.MethodName == "ProcessScore")
            //{
            //    auditInfo.ClientName = ActionReport.Student_Statistics.ToString();
            //    auditInfo.CustomData = string.Format("Course Completion: {0}", auditInfo.Parameters);
            //    return base.SaveAsync(auditInfo);
            //}

            //Student_Statistics_Course_Assignment_completed = 23, StudentAssignmentFile
            if (auditInfo.ServiceName.Contains("StudentAssignmentFile") && auditInfo.MethodName == "Create")
            {
                string fileName = getNameFromJson(auditInfo.Parameters, "fileName");
                auditInfo.ClientName = ActionReport.Student_Statistics_Course_Assignment_completed.ToString();
                auditInfo.CustomData = string.Format("Submitted Assignment: {0}", fileName);
                return base.SaveAsync(auditInfo);
            }


            //Student_Statistics_Course_Total_Scores = 25,

            if (auditInfo.ServiceName == "RMALMS.Courses.CourseAppService")
            {
                switch (auditInfo.MethodName)
                {
                    case "Create":
                    case "Update":
                    case "UpdateCourseAssignedStudentStatus":
                    case "CreateOrUpdateCourseLMSSetting":
                    case "AddStudentsToCourse":
                        break;
                    default:
                        auditInfo.ClientName = ActionReport.Student_Statistics.ToString();
                        auditInfo.CustomData = string.Format("Course: {0}", auditInfo.MethodName);
                        return base.SaveAsync(auditInfo);
                }

            }

            #endregion  Student_Statistics

            #region Instructor_Statistics


            //Instructor_Statistics_Report_download = 31,
            // TODO: Done


            if (auditInfo.ServiceName == "RMALMS.Courses.CourseAppService")
            {
                switch (auditInfo.MethodName)
                {
                    //Instructor_Statistics_Created_Course = 32,
                    case "Create":
                        auditInfo.ClientName = ActionReport.Instructor_Statistics_Created_Course.ToString();
                        string name = getNameFromJson(auditInfo.Parameters, "name");
                        auditInfo.CustomData = string.Format("Drafted the course (Course name: {0})", name);
                        return base.SaveAsync(auditInfo);
                    //Instructor_Statistics_Published_course = 33, + Course_Statistics_Published_Course
                    case "Update":
                        string state = getNameFromJson(auditInfo.Parameters, "state");
                        if (state == "1")
                        {
                            auditInfo.ClientName = ActionReport.Instructor_Statistics_Published_course.ToString()
                                + "; " + ActionReport.Course_Statistics_Published_Course.ToString();
                            state = getNameFromJson(auditInfo.Parameters, "identifier");
                            auditInfo.CustomData = string.Format("Published the course (Course Code: {0})", state);
                            return base.SaveAsync(auditInfo);
                        }
                        break;
                    //Instructor_Statistics_Course_enrolled_and_invited_student = 34, + Course_Statistics_Course_enrolled_and_invited_student
                    // Accept course enrolled
                    case "UpdateCourseAssignedStudentStatus":
                        string status = getNameFromJson(auditInfo.Parameters, "status");
                        if (status == "2")
                        {
                            status = getNameFromJson(auditInfo.Parameters, "userName");
                            auditInfo.ClientName = ActionReport.Instructor_Statistics_Course_enrolled_and_invited_student.ToString()
                                + "; " + ActionReport.Course_Statistics_Course_enrolled_and_invited_student.ToString();
                            auditInfo.CustomData = string.Format("Accept {0} to course enrolled", status);
                            return base.SaveAsync(auditInfo);
                        }
                        else if (status == "3")
                        {
                            status = getNameFromJson(auditInfo.Parameters, "userName");
                            auditInfo.ClientName = ActionReport.Instructor_Statistics_Course_enrolled_and_invited_student.ToString()
                                + "; " + ActionReport.Course_Statistics_Course_enrolled_and_invited_student.ToString();
                            auditInfo.CustomData = string.Format("Reject {0} to course enrolled", status);
                            return base.SaveAsync(auditInfo);
                        }
                        break;

                }

            }
            // Invited  student
            if (auditInfo.MethodName == "AddStudentsToCourse")
            {
                string names = getNameFromJson(auditInfo.Parameters, "studentNames");
                auditInfo.ClientName = ActionReport.Instructor_Statistics_Course_enrolled_and_invited_student.ToString();
                //+ "; " + ActionReport.Course_Statistics_Course_enrolled_and_invited_student.ToString();
                auditInfo.CustomData = string.Format("Invited {0}", names);
                return base.SaveAsync(auditInfo);
            }

            //Instructor_Statistics_Update_student_performance = 35,

            //Instructor_Statistics_Course_Features = 36,
            // TODO: Done

            #endregion Instructor_Statistics

            #region Course_Statistics
            //Course_Statistics_Published_Course = 41,
            //TODO: done

            //Course_Statistics_Course_Instructors = 42,
            if (auditInfo.ServiceName == "RMALMS.UserExtraRoles.UserExtraRoleAppService" && auditInfo.MethodName == "AddCourseAdminsToCourse")
            {
                string names = getNameFromJson(auditInfo.Parameters, "courseAdminNames");
                if (names == "")
                {
                    auditInfo.CustomData = string.Format("Remove all teacher as course instructor");
                }
                else
                {
                    auditInfo.CustomData = string.Format("Added \"{0}\" as course instructor", names);
                }
                auditInfo.ClientName = ActionReport.Course_Statistics_Course_Instructors.ToString();
                return base.SaveAsync(auditInfo);
            }

            //Course_Statistics_Course_Announcement = 43,
            if (auditInfo.ServiceName == "RMALMS.Annoucements.AnnoucementAppService" && auditInfo.MethodName == "Create")
            {
                string title = getNameFromJson(auditInfo.Parameters, "title");
                auditInfo.CustomData = string.Format("Posted announcement, title as \"{0}\"", title);
                auditInfo.ClientName = ActionReport.Course_Statistics_Course_Announcement.ToString();
                return base.SaveAsync(auditInfo);
            }

            //Course_Statistics_Course_Options_and_Features = 44,
            if (auditInfo.ServiceName == "RMALMS.Courses.CourseAppService" && auditInfo.MethodName == "CreateOrUpdateCourseLMSSetting")
            {
                auditInfo.CustomData = string.Format("Change Course Feature Option");
                auditInfo.ClientName = ActionReport.Course_Statistics_Course_Options_and_Features.ToString()
                     + "; " + ActionReport.Instructor_Statistics_Course_Features.ToString();
                return base.SaveAsync(auditInfo);
            }

            //Course_Statistics_Course_enrolled_and_invited_student = 45,
            //TODO: done

            //Course_Statistics_Update_student_performance = 46,
            // TODO: ...
            #endregion Course_Statistics

            #region Course_Import_Export
            //Course_Import_Export_Course_Import = 51,
            // TODO: ...

            //Course_Import_ExportCourse_Export = 52,
            // TODO: ...

            #endregion Course_Import_Export



            return base.SaveAsync(auditInfo);
        }


        #region Private
        private Guid getIdFromJson(string json, string linkTableId)
        {
            try
            {
                dynamic stuff = JsonConvert.DeserializeObject(json);
                if (json.Contains("input"))
                {
                    string linkId = stuff.input[linkTableId];
                    return Guid.Parse(linkId);
                }
                else if (json.Contains("pageId"))
                {
                    string linkId = stuff.pageId[linkTableId];
                    return Guid.Parse(linkId);
                }
                else
                {
                    string linkId = stuff[linkTableId];
                    return Guid.Parse(linkId);
                }

            }
            catch (Exception)
            {
                return Guid.Empty;
            }

        }
        private string getNameFromJson(string json, string linkTableId)
        {
            try
            {
                dynamic stuff = JsonConvert.DeserializeObject(json);
                if (json.Contains("input"))
                {
                    var result = stuff.input[linkTableId];

                    if (result.ToString().Contains("["))
                    {
                        return string.Join("; ", result);
                    }
                    else
                        return result.ToString();
                }
                else if (json.Contains("pageId"))
                {
                    return stuff.pageId[linkTableId];
                }
                else
                {
                    return stuff[linkTableId];
                }
            }
            catch (Exception)
            {
                return json;
            }

        }

        private string GetCourseNameFromCourseInstanceId(Guid courseInstanceId)
        {
            if (courseInstanceId == Guid.Empty) return null;
            var qCourseInstance = from courseIns in _ws.GetRepo<CourseInstance, Guid>().GetAll().Where(p => p.Id == courseInstanceId)
                                  join course in _ws.GetRepo<Course, Guid>().GetAll()
                                  on courseIns.CourseId equals course.Id
                                  select course.Name;
            return qCourseInstance.FirstOrDefault();
        }

        private PageCourseDto GetPageAndCourseNameFromPageId(Guid id)
        {
            if (id == Guid.Empty) return new PageCourseDto();
            var qPage = from page in _ws.GetRepo<Page, Guid>().GetAll().Where(p => p.Id == id)
                        join course in _ws.GetRepo<Course, Guid>().GetAll()
                        on page.CourseId equals course.Id
                        select new PageCourseDto
                        {
                            CourseName = course.Name,
                            PageName = page.Name,
                        };
            return qPage.FirstOrDefault();
        }
        #endregion Private
    }
}

public class PageCourseDto
{
    public string PageName { get; set; }
    public string CourseName { get; set; }
}

