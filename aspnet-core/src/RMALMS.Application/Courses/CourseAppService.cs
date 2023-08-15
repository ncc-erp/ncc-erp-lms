using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMALMS.Courses.Dto;
using RMALMS.Entities;
using RMALMS.Extension;
using RMALMS.Helper;
using RMALMS.IoC;
using RMALMS.Paging;
using RMALMS.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using RMALMS.DomainServices;
using RMALMS.Authorization.Roles;
using Abp.Authorization.Users;
using RMALMS.Authorization.Users;
using RMALMS.Users.Dto;
using RMALMS.Common;
using RMALMS.Authorization.Accounts.Dto;
using Abp.UI;
using RMALMS.Assignments.Dto;
using RMALMS.DomainServices.Entity;
using RMALMS.Uitls;
using System.IO.Compression;
using System.IO;
using Abp.Configuration;
using RMALMS.Configuration;
using System.Xml;

namespace RMALMS.Courses
{
    [AbpAuthorize]
    public class CourseAppService : AsyncCrudAppService<Course, EditCourseDto, Guid, PagedResultRequestDto, CreateCourseDto, EditCourseDto>, ICourseAppService
    {
        private readonly IWorkScope _ws;
        private readonly IUploadHelper _uploadHelper;
        private readonly ICourseManager _courseManager;
        private readonly IUserServices _userService;
        private IUserCertificationManager _userCertificationManager;

        public CourseAppService(
            IRepository<Course, Guid> repository,
            IWorkScope workScope,
            IUploadHelper uploadHelper,
            ICourseManager courseManager,
            IUserServices userService,
            IUserCertificationManager userCertificationManager
        ) : base(repository)
        {
            _ws = workScope;
            this._uploadHelper = uploadHelper;
            this._courseManager = courseManager;
            _userService = userService;
            _userCertificationManager = userCertificationManager;
        }

        public async Task CompletedCourse(Guid courseAssignedStudentId)
        {
            var courseAssignedStudent = await _ws.GetRepo<CourseAssignedStudent>()
                .GetAllIncluding(s => s.CourseInstance, s => s.CourseInstance.Course)
                .Where(s => s.Id == courseAssignedStudentId)
                .FirstOrDefaultAsync();
            if (courseAssignedStudent == default)
            {
                throw new UserFriendlyException("Not found course assigned student");
            }
            //var courseAssignedStudent = await _ws.GetRepo<CourseAssignedStudent>().GetAsync(courseAssignedStudentId);            
            if (courseAssignedStudent != null)
            {
                courseAssignedStudent.Status = AssignedStatus.Completed;
                await _ws.UpdateAsync(courseAssignedStudent);
            }
            var courseInstanceId = courseAssignedStudent.CourseInstanceId;
            var studentId = courseAssignedStudent.StudentId;
            var courseSourse = courseAssignedStudent.CourseInstance.Course.Sourse;
            UserCertification cer = null;
            if (courseSourse == CourseSource.RMA)
            {
                cer = await _userCertificationManager.CreateUpdateUserCertification(courseAssignedStudentId, CertificationType.Completion, UpdateUserCertificationOption.UpdateIfNotExistInsert);
            }
            else
            {
                cer = await _userCertificationManager.CreateUpdateUserCertificationScorm(courseAssignedStudentId, CertificationType.Completion, UpdateUserCertificationOption.UpdateIfNotExistInsert);
            }

            //update student level
            var studentLevel = await _ws.GetRepo<User, long>().GetAllIncluding(s => s.Status)
                .Where(s => s.Id == courseAssignedStudent.StudentId).Select(s => new { User = s, s.Status })
                .FirstOrDefaultAsync();

            if (studentLevel != null && studentLevel.Status != null)
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                var courseLevelCount = await (from c in _ws.GetAll<Course>()
                                              join cl in _ws.GetAll<CourseLevel>().Where(s => s.Level == studentLevel.Status.Level) on c.LevelId equals cl.Id
                                              join cc in
                                                  from ci in _ws.GetAll<CourseInstance>()
                                                  join cas in _ws.GetAll<CourseAssignedStudent>().Where(s => s.StudentId == studentId && s.Status == AssignedStatus.Completed)
                                                  on ci.Id equals cas.CourseInstanceId
                                                  select ci.CourseId
                                              on c.Id equals cc
                                              select c.Id
                            ).Distinct().CountAsync();

                if ((studentLevel.Status.LowCompareOperation == CompareOperation.GreaterThan && courseLevelCount > studentLevel.Status.RequiredNumber)
                    || (studentLevel.Status.LowCompareOperation == CompareOperation.GreaterEqual && courseLevelCount >= studentLevel.Status.RequiredNumber))
                {
                    //studentLevel.s.StatusId = 
                    var orderedLevels = await _ws.GetAll<Entities.UserStatus>().OrderBy(s => s.Level).Select(s => new { s.Level, s.Id }).ToListAsync();
                    //small -> big by level
                    foreach (var level in orderedLevels)
                    {
                        if (level.Level > studentLevel.Status.Level)
                        {
                            studentLevel.User.StatusId = level.Id;
                            await _ws.UpdateAsync<User, long>(studentLevel.User);
                            break;
                        }
                    }
                }
            }

        }

        [HttpPost]
        public async Task<GridResult<CourseDto>> GetAllPagging(GridParam input)
        {

            bool isSystemAdmin = _userService.UserHasRole(AbpSession.UserId.Value, StaticRoleNames.Tenants.Admin);

            var query = from ci in _ws.GetRepo<CourseInstance>().GetAllIncluding(ci => ci.Course)
                        join cl in _ws.GetAll<CourseLevel, Guid>() on ci.Course.LevelId equals cl.Id into levels
                        from l in levels.DefaultIfEmpty().Take(1)
                        where (isSystemAdmin ? true : ci.CreatorUserId == AbpSession.UserId)
                        select new CourseDto()
                        {
                            Id = ci.Id,
                            CourseId = ci.CourseId,
                            Name = ci.Course.Name,
                            ImageCover = ci.Course.ImageCover,
                            Level = l.DisplayName,
                            State = ci.Course.State
                        };

            return await query.GetGridResult(query, input);
        }





        public ListResultDto<PermissionDto> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();
            return new ListResultDto<PermissionDto>();
        }

        public async Task<CourseScormUploadDto> ScormUpload([FromForm] FileSCORMInput input)
        {

            if (input.File != null)
            {
                var zipFile = await _uploadHelper.UploadFile(input.File, DateTime.Now.ToString("yyMMdd_HHmmssff"));

                if (Path.GetExtension(zipFile.FileName).Equals(".zip"))
                {
                    var extractFolder = Path.GetDirectoryName(zipFile.FilePath);
                    ZipFile.ExtractToDirectory(zipFile.FilePath, extractFolder);
                    var manifestXml = Path.Combine(extractFolder, "imsmanifest.xml");
                    if (!File.Exists(manifestXml))
                    {
                        // Delete temp files
                        await Task.Factory.StartNew(path => Directory.Delete((string)path, true), extractFolder);
                        throw new UserFriendlyException("File does not support");
                    }
                    XmlTextReader textReader = new XmlTextReader(manifestXml);
                    CourseScormUploadDto scormOut = new CourseScormUploadDto();
                    string version = "";

                    while (textReader.Read())
                    {
                        // Move to fist element  
                        textReader.MoveToElement();
                        if (textReader.Name == "schemaversion")
                        {
                            version = textReader.ReadString();
                            // Check version supported
                            if (version.Contains("1.2"))
                            {
                                scormOut.SourseVersion = CourseSource.Scorm12;
                            }
                            else if (version.Contains("2004"))
                            {
                                scormOut.SourseVersion = CourseSource.Scorm2004;
                            }
                            else
                            {
                                // Delete temp files
                                textReader.Close();
                                await Task.Factory.StartNew(path => Directory.Delete((string)path, true), extractFolder);
                                throw new UserFriendlyException("The Version of SCORM does not support");
                            }
                        }
                        if (textReader.Name == "title")
                        {
                            scormOut.Title = textReader.ReadString();
                            break;
                        }
                    }
                    // Delete temp files
                    textReader.Close();
                    await Task.Factory.StartNew(path => Directory.Delete((string)path, true), extractFolder);
                    if (string.IsNullOrEmpty(scormOut.Title) || string.IsNullOrEmpty(version))
                    {
                        throw new UserFriendlyException("The Version of SCORM does not support");
                    }
                    return scormOut;
                }
                throw new UserFriendlyException("Only allow zip file");
            }
            throw new UserFriendlyException("File is null");

        }


        public async override Task<EditCourseDto> Create([FromForm] CreateCourseDto input)
        {
            CheckCreatePermission();

            //create new Course and get CourseId
            var item = ObjectMapper.Map<Course>(input);
            if (item.Identifier == null || item.Identifier.IsEmpty())
            {
                item.Identifier = DateTime.Now.ToFileTimeUtc().ToString();
            }

            item.Id = await _ws.InsertAndGetIdAsync(item);
            await CurrentUnitOfWork.SaveChangesAsync();
            //upload ImageCover 
            if (input.File != null)
            {
                string postfix = string.Empty;
                string folder = string.Empty;
                folder = "Course";
                var id = typeof(Guid).ChangeType(item.Id) as Guid?;
                if (id.HasValue)
                {
                    postfix = id.Value.ToString();
                }
                postfix = postfix.Length > 0 ? $"-{postfix}" : postfix;
                folder = $"{folder}{postfix}";

                var img = await _uploadHelper.UploadFile(input.File, folder);
                item.ImageCover = img.ServerPath;

                await _ws.GetRepo<Course>().UpdateAsync(item);
            }

            if (input.FileSCORM != null)
            {
                string scormFolder = "SCORM";
                scormFolder = $"{scormFolder}/{AbpSession.TenantId.ToString()}/{item.Id}";
                var zipFile = await _uploadHelper.UploadFile(input.FileSCORM, scormFolder, isSCORM: true);
                if (Path.GetExtension(zipFile.FileName).Equals(".zip"))
                {
                    var extractFolder = Path.GetDirectoryName(zipFile.FilePath);
                    ZipFile.ExtractToDirectory(zipFile.FilePath, extractFolder);
                    item.SoursePath = $"{zipFile.ServerPath.Replace(zipFile.FileName, "")}shared/launchpage.html";
                }

                await _ws.GetRepo<Course>().UpdateAsync(item);
            }

            //create course setting (course instant)
            await this._courseManager.CreateCourseInstance(item.Id);
            await this._courseManager.CreateCourseDefaultTemplate(item.Id);
            return ObjectMapper.Map<EditCourseDto>(item);

        }

        public async override Task<EditCourseDto> Update([FromForm] EditCourseDto input)
        {
            CheckUpdatePermission();
            //var item = Repository.Get(input.Id);
            var item = await _ws.GetRepo<Course>().GetAsync(input.Id);
            ObjectMapper.Map(input, item);
            //MapToEntity(input, item);

            //upload ImageCover 
            if (input.File != null)
            {
                string postfix = string.Empty;
                string folder = string.Empty;
                folder = "Course";
                var id = typeof(Guid).ChangeType(item.Id) as Guid?;
                if (id.HasValue)
                {
                    postfix = id.Value.ToString();
                }
                postfix = postfix.Length > 0 ? $"-{postfix}" : postfix;
                folder = $"{folder}{postfix}";

                var img = await _uploadHelper.UploadFile(input.File, folder);
                item.ImageCover = img.ServerPath;

                //await _ws.GetRepo<Course>().UpdateAsync(item);
            }

            //upload Related Image
            if (input.RelatedFile != null)
            {
                string postfix = string.Empty;
                string folder = string.Empty;
                folder = "Course";
                var id = typeof(Guid).ChangeType(item.Id) as Guid?;
                if (id.HasValue)
                {
                    postfix = id.Value.ToString();
                }
                postfix = postfix.Length > 0 ? $"-{postfix}" : postfix;
                folder = $"{folder}{postfix}";

                var img = await _uploadHelper.UploadFile(input.RelatedFile, folder);
                item.RelatedImage = img.ServerPath;

                //await _ws.GetRepo<Course>().UpdateAsync(item);
            }

            await _ws.UpdateAsync(item);

            return input;
        }

        public async Task<EditCourseDto> GetByCourseInstanceId(EntityDto<Guid> input)
        {
            var item = await _ws.GetRepo<CourseInstance>().GetAllIncluding(ci => ci.Course).Where(ci => ci.Id == input.Id).Select(ci => new EditCourseDto()
            {
                Id = ci.CourseId,
                Name = ci.Course.Name,
                Identifier = ci.Course.Identifier,
                Description = ci.Course.Description,
                ImageCover = ci.Course.ImageCover,
                RelatedImage = ci.Course.RelatedImage,
                RelatedInformation = ci.Course.RelatedInformation,
                LanguageId = ci.Course.LanguageId,
                LevelId = ci.Course.LevelId,
                RestrictStudentFromViewThisCourseAfterEndDate = ci.Course.RestrictStudentFromViewThisCourseAfterEndDate,
                RestrictStudentsFromViewingThisCourseBeforeEndDate = ci.Course.RestrictStudentsFromViewingThisCourseBeforeEndDate,
                StudentCanOnlyParticipiateCourseBetweenTheseDate = ci.Course.StudentCanOnlyParticipiateCourseBetweenTheseDate,
                Type = ci.Course.Type,
                Syllabus = ci.Course.Syllabus,
                State = ci.Course.State,
                Sourse = ci.Course.Sourse,
                SoursePath = ci.Course.SoursePath,
            }).FirstOrDefaultAsync();
            return item;
        }

        public async Task<Object> GetScormCourseAndCourseAssignedStudent(Guid courseInstanceId)
        {
            //var courseAssignedStudent = await _ws.GetAll<CourseAssignedStudent>()
            //    .Where(s => s.CourseInstanceId == courseInstanceId && s.StudentId == AbpSession.UserId && s.Status == AssignedStatus.Accepted)
            //    .LastOrDefaultAsync();
            var query = from ci in _ws.GetAll<CourseInstance>().Where(s => s.Id == courseInstanceId)
                        join c in _ws.GetAll<Course>() on ci.CourseId equals c.Id
                        join cas in _ws.GetAll<CourseAssignedStudent>()
                        .Where(s => s.CourseInstanceId == courseInstanceId && s.StudentId == AbpSession.UserId.Value) on ci.Id equals cas.CourseInstanceId into cases
                        from cas in cases.DefaultIfEmpty().Take(1)
                        select new
                        {
                            CourseAssignedStudentId = cas != null ? cas.Id : (Guid?)null,
                            c.SoursePath,
                            CourseInstanceId = ci.Id,
                            c.Sourse,
                            Status = cas != null ? cas.Status : (AssignedStatus?) null
                        };

            return await query.FirstOrDefaultAsync();

            //var item = await _ws.GetRepo<CourseInstance>().GetAllIncluding(ci => ci.Course)
            //    .Where(ci => ci.Id == courseInstanceId)
            //    .Select(ci => new {
            //        CourseAssignedStudentId = courseAssignedStudent != null ? courseAssignedStudent.Id : (Guid?) null,
            //        SoursePath = ci.Course.SoursePath,
            //        CourseInstanceId = ci.Id,
            //}).FirstOrDefaultAsync();
            //return item;
        }

        public async Task<ListResultDto<CourseStatusDto>> GetAllStatus()
        {
            var query = _ws.GetRepo<CourseLevel>().GetAll().ProjectTo<CourseStatusDto>();
            var items = await query.ToListAsync();
            return new ListResultDto<CourseStatusDto>(items);
        }

        #region Course People
        [HttpPost]
        public async Task<GridResult<AssignedStudentCourseDto>> GetAssignedStudentByCourseAndStatus(RequestAssignedStudentCourseDto requestDto)
        {
            IQueryable<CourseAssignedStudent> qassignedstudents = null;
            //var qassignedstudents = _courseManager.GetStudentAssignedCourseByStatus(requestDto.CourseInstanceId, requestDto.Status);
            if (requestDto.Status == AssignedStatus.Accepted)
            {
                qassignedstudents = _courseManager.GetCourseAssignedStudentsAcceptedAndCompleted(requestDto.CourseInstanceId);
            }
            else
            {
                qassignedstudents = _courseManager.GetStudentAssignedCourseByStatus(requestDto.CourseInstanceId, requestDto.Status);
            }

            var qusers = GetAssignStudents(qassignedstudents);
            return await qusers.GetGridResult(qassignedstudents, requestDto.Request);
        }

        [HttpPost]
        public async Task<GridResult<AssignedStudentCourseDto>> GetInvitationStudentByCourse(InvitationCourseRequestDto requestDto)
        {
            // get direct assigned student
            var qallAssignedStudents = _courseManager.GetStudentAssignedCourseByStatus(requestDto.CourseInstanceId, null, false);
            var qinvitedStudents = qallAssignedStudents.Where(s => s.Status == AssignedStatus.Invited);

            var alreadyAssingedStudentId = qallAssignedStudents.Select(s => s.StudentId);

            // get students are assigned via group
            var qinvitedStudentIds = qinvitedStudents.Select(s => s.StudentId);
            var assignedGroupIds =
                from g in _ws.GetAll<GroupAssignedCourse>().Where(s => s.CourseInstanceId == requestDto.CourseInstanceId)
                select g.GroupId;

            // ignore user already assigned to system
            var assignStudentIds =
                from ug in _ws.GetAll<UserGroup>()
                join g in assignedGroupIds on ug.GroupId equals g into groups
                join u in alreadyAssingedStudentId on ug.UserId equals u into students
                where groups.Any() && !students.Any()
                select ug.UserId;

            var qassignedstudents =
                from u in
                    from user in _ws.GetAll<User, long>()
                    join au in assignStudentIds on user.Id equals au into students
                    join inu in qinvitedStudentIds on user.Id equals inu into invitedstudents
                    where students.Any() || invitedstudents.Any()
                    select user
                join r in
                    from r in _ws.GetAll<Role, int>()
                    join us in _ws.GetAll<UserRole, long>() on r.Id equals us.RoleId
                    select new { r.Name, us.UserId }
                 on u.Id equals r.UserId into roles

                select new AssignedStudentCourseDto
                {
                    StudentName = u.Name + " " + u.Surname,
                    StudentId = u.Id,
                    UserName = u.UserName,
                    Roles = roles.Select(s => s.Name)
                };

            return await qassignedstudents.GetGridResult(qassignedstudents, requestDto.Request);
        }

        private IQueryable<AssignedStudentCourseDto> GetAssignStudents(IQueryable<CourseAssignedStudent> qassignedstudents)
        {
            var qusers =
               from sa in qassignedstudents
               join r in
                   from r in _ws.GetAll<Role, int>()
                   join us in _ws.GetAll<UserRole, long>() on r.Id equals us.RoleId
                   select new { r.Name, us.UserId }
                on sa.StudentId equals r.UserId into roles
               select new AssignedStudentCourseDto
               {
                   StudentName = sa.Student.DisplayName,
                   StudentId = sa.StudentId,
                   StudentAssignedCourseId = sa.Id,
                   UserName = sa.Student.UserName,
                   Roles = roles.Select(s => s.Name),
                   EnrollCount = sa.EnrollCount,
                   LastActivity = sa.CreationTime,
                   Status = sa.Status
               };

            return qusers;
        }

        [HttpPost]
        public async Task<GridResult<SelectableStudentDto>> GetUnAssingedStudents(UnAssignedStudentCourseDto input)
        {
            var qallStudents = _userService.GetUserByRole(StaticRoleNames.Tenants.Student);
            var qassingStudentIds = _courseManager.GetAllStudentAssignedToCourse(input.CourseInstanceId).Select(s => s.Id);

            var qunassignedStudents =
                from u in qallStudents
                join a in qassingStudentIds on u.Id equals a into assignedStudents
                where !assignedStudents.Any()
                select new SelectableStudentDto
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName == null || u.DisplayName == "" ? u.UserName : u.DisplayName,
                    FirstName = u.Name,
                    LastName = u.Surname
                };

            return await qunassignedStudents.GetGridResult(qunassignedStudents, input.Request);
        }

        public async Task<CourseAssignedStudentDto> UpdateCourseAssignedStudentStatus(UpdateCourseAssignedStudentStatusDto input)
        {
            var item = await _ws.GetRepo<CourseAssignedStudent>().GetAsync(input.Id);
            item.Status = input.Status;
            await _ws.UpdateAsync(item);
            return ObjectMapper.Map<CourseAssignedStudentDto>(item);
        }

        #endregion

        #region Dashboard for Student
        // /// <summary>
        // /// Get All published Course For Student dashboard
        // /// </summary>
        // /// <returns></returns>
        [HttpPost]
        [AbpAuthorize(Authorization.PermissionNames.Pages_Dashboard)]
        public async Task<GridResult<CourseDashboardDto>> GetAllCourse([FromBody]GridParam input)
        {
            var studentStatus = await _ws.GetRepo<User, long>().GetAllIncluding(s => s.Status).Where(s => s.Id == AbpSession.UserId).Select(s => s.Status).FirstOrDefaultAsync();
            var studentLevel = studentStatus != null ? studentStatus.Level : 0;

            var Now = DateTime.UtcNow;
            var ComminSoonDate = DateTime.UtcNow.AddDays(14);
            var courseProgress = _ws.GetRepo<StudentProgress>().GetAllIncluding(s => s.Page)
                                 .Where(s => s.CreatorUserId == AbpSession.UserId.Value && s.Progress == StudentProgressStatus.Completed && s.Page.Type == PageType.Page)
                                 .GroupBy(s => s.CourseInstanceId)
                                 .Select(s => new { CourseInstanceId = s.Key, Progress = s.Count() });
            //var qcourseAssignedStudent = _ws.GetAll<CourseAssignedStudent>().Where( s=> s.StudentId == AbpSession.UserId)
            var qStudentProficiency = await _ws.GetAll<Setting, long>().Where(d => d.Name == AppSettingNames.StudentProficiencyLevelRequired).FirstOrDefaultAsync();
            var requiredStudentLevel = Boolean.Parse(qStudentProficiency != null ? qStudentProficiency.Value ?? "False" : "False");
            var qcourseDashboard =
                (from ci in _ws.GetAll<CourseInstance>().Where(cci => (!cci.EndTime.HasValue || cci.EndTime.Value >= Now) && cci.Status == CourseSettingStatus.Active)
                 join cas in _ws.GetAll<CourseAssignedStudent>().Where(s => s.StudentId == AbpSession.UserId.Value && s.Status != AssignedStatus.Completed && s.Status != AssignedStatus.Accepted) on ci.Id equals cas.CourseInstanceId into statuses
                 join cp in courseProgress on ci.Id equals cp.CourseInstanceId into progresses
                 join c in _ws.GetAll<Course>().Where(cc => cc.State == CourseState.Publish) on ci.CourseId equals c.Id
                 join p in _ws.GetAll<Page>().Where(s => s.Type == PageType.Page) on c.Id equals p.CourseId into pages
                 join cl in _ws.GetAll<CourseLevel>() on c.LevelId equals cl.Id
                 from progress in progresses.DefaultIfEmpty().Take(1)
                 from status in statuses.DefaultIfEmpty().Take(1)
                 let AlreadyStart = ci.StartTime <= Now && ci.EndTime >= Now
                 let IsCommingSoon = ci.StartTime <= ComminSoonDate && ci.StartTime > Now
                 let IsUpComming = ci.StartTime.HasValue && ci.StartTime.Value > ComminSoonDate
                 where !requiredStudentLevel || (!cl.RequiredStudentLevel.HasValue
                 || !cl.LowCompareOperation.HasValue
                 || (cl.LowCompareOperation == CompareOperation.Equal && studentLevel == cl.RequiredStudentLevel)
                 || (cl.LowCompareOperation == CompareOperation.GreaterEqual && studentLevel >= cl.RequiredStudentLevel)
                 || (cl.LowCompareOperation == CompareOperation.GreaterThan && studentLevel > cl.RequiredStudentLevel)
                 || (cl.LowCompareOperation == CompareOperation.LessEqual && studentLevel <= cl.RequiredStudentLevel)
                 || (cl.LowCompareOperation == CompareOperation.LessThan && studentLevel < cl.RequiredStudentLevel)
                 )
                 select new CourseDashboardDto
                 {
                     Id = ci.Id,
                     StartDate = ci.StartTime,
                     EndDate = ci.EndTime,
                     Name = c.Name,
                     Description = c.Description,
                     RelationInfo = c.RelatedInformation,
                     CurrentPoint = AlreadyStart ? 100 : 0,
                     ComminSoonDatePoint = IsCommingSoon ? 10 : 0,
                     AlreadyStart = AlreadyStart,
                     IsCommingSoon = IsCommingSoon,
                     IsUpComming = IsUpComming,
                     IsSelfPaced = !ci.NumberDayToStudy.HasValue,
                     ImageCover = c.ImageCover,
                     State = c.State,
                     Status = status != null ? status.Status : (AssignedStatus?)null,
                     NCompletedPage = progress != null ? progress.Progress : 0,
                     TotalPage = pages.Count(),
                 }).OrderBy(s => s.CurrentPoint + s.ComminSoonDatePoint).ThenByDescending(s => s.StartDate);

            return await qcourseDashboard.GetGridResult(qcourseDashboard, input);
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_Dashboard)]
        public async Task<StatictisCourseDto> GetCourseStatistic()
        {
            var Now = DateTime.UtcNow;
            var ComminSoonDate = DateTime.UtcNow.AddDays(14);

            var qcourses =
                from ci in _ws.GetAll<CourseInstance>()
                join c in _ws.GetAll<Course>() on ci.CourseId equals c.Id
                where ci.Status == CourseSettingStatus.Active
                select new
                {
                    c.Id,
                    ci.StartTime,
                    ci.EndTime,
                    c.LevelId
                };

            var qcourseDashboard =
                from c in qcourses

                let AlreadyStart = c.StartTime.HasValue && c.EndTime.HasValue && c.StartTime <= Now && c.EndTime >= Now
                let IsCommingSoon = c.StartTime.HasValue && c.StartTime <= ComminSoonDate && c.StartTime > Now
                let IsUpComming = c.StartTime.HasValue && c.StartTime.Value > ComminSoonDate
                let IsExpried = c.EndTime.HasValue && c.EndTime < Now

                select new
                {
                    c.Id,
                    Current = AlreadyStart ? 1 : 0,
                    CommingSoon = IsCommingSoon ? 1 : 0,
                    UpComming = IsUpComming ? 1 : 0,
                    Expried = IsExpried ? 1 : 0
                };

            var qcourseWithLevel =

                from l in _ws.GetAll<CourseLevel>()
                join c in qcourses on l.Id equals c.LevelId into courses
                select new StatictisCourseLevel { LevelName = l.DisplayName, Total = courses.Count() };

            var qcourseWithCategory =
                from cat in _ws.GetAll<Category>()
                join ct in
                    from c in qcourses
                    join ct in _ws.GetAll<CourseTag>() on c.Id equals ct.CourseId
                    select ct.CategoryId
                on cat.Id equals ct into courses
                select new StatictisCourseCategory { CategoryName = cat.Name, Total = courses.Count() };

            var qcourseWithStatictis =
                from c in qcourseDashboard
                group c by c.Id into g
                select new
                {
                    TotalCurrent = g.Sum(s => s.Current),
                    TotalCommingSoon = g.Sum(s => s.CommingSoon),
                    TotalUpComming = g.Sum(s => s.UpComming),
                    TotalExpried = g.Sum(s => s.Expried)
                };

            // query database
            var courseWithStatictis = await qcourseWithStatictis.FirstOrDefaultAsync();
            var courseWithCategory = await qcourseWithCategory.ToListAsync();
            var courseWithLevel = await qcourseWithLevel.ToListAsync();

            return new StatictisCourseDto()
            {
                TotalCurrent = courseWithStatictis != null ? courseWithStatictis.TotalCurrent : 0,
                TotalExpired = courseWithStatictis != null ? courseWithStatictis.TotalExpried : 0,
                TotalUpComming = courseWithStatictis != null ? courseWithStatictis.TotalUpComming : 0,
                TotalStartSoon = courseWithStatictis != null ? courseWithStatictis.TotalCommingSoon : 0,
                StatictisCourseCategory = courseWithCategory,
                StatictisCourseLevels = courseWithLevel
            };
        }

        public async Task<CourseInstructorDto> GetProfileById(long Id)
        {
            var userDto = new CourseInstructorDto();
            var quserCertifications =
               from cer in _ws.GetAll<UserCertification>().Where(s => s.CourseAssignedStudent.StudentId == AbpSession.UserId)
               join ci in _ws.GetAll<CourseInstance>() on cer.CourseInstanceId equals ci.CourseId
               join c in _ws.GetAll<Course>() on ci.CourseId equals c.Id
               join cl in _ws.GetAll<CourseLevel>() on c.LevelId equals cl.Id
               select new { LevelId = cl.Id, LevelName = cl.DisplayName, CourseId = c.Id, UserId = cer.CourseAssignedStudent.StudentId };

            var ggroupUserCeritifications =
                from uc in quserCertifications
                group uc by new { uc.LevelId, uc.LevelName, uc.UserId } into g
                select new { g.Key.LevelName, g.Key.UserId, TotalCourse = g.Count() };

            var query =
               from u in _ws.GetAll<User, long>().Where(us => us.Id == Id)
               join l in _ws.GetAll<UserLink>() on u.Id equals l.CreatorUserId into links
               join uc in ggroupUserCeritifications on u.Id equals uc.UserId into archs
               select new { User = u, Archievements = archs, UserLinks = links };
            var user = await query.FirstOrDefaultAsync();
            userDto.Name = user.User.FullName;
            userDto.FullName = user.User.FullName;
            userDto.Avatar = user.User.Avatar;
            if (user.User.UserPersonalInfoViewByPublic)
            {
                userDto.EmailAddress = user.User.EmailAddress;
                userDto.Title = user.User.Title;
                userDto.DisplayName = user.User.DisplayName;
            }
            if (user.User.UserPersonalLinksViewByPublic)
            {
                userDto.UserLinks = ObjectMapper.Map<IEnumerable<UserLinkDto>>(user.UserLinks.ToList());
            }
            if (user.User.UserPersonalAchievementViewByPublic)
            {
                userDto.Achievement = ObjectMapper.Map<IEnumerable<AchievementDto>>(user.Archievements.ToList());
            }
            if (user.User.UserPersonalCertificationViewByPublic)
            {

            }
            return userDto;
        }

        public async Task<object> GetCourseByInstanceId(Guid courseInstanceId, Guid? courseAssignedStudentId)
        {

            var qalreadyAssingedStudent = _ws.GetAll<CourseAssignedStudent>().Where(s => s.StudentId == AbpSession.UserId && s.CourseInstanceId == courseInstanceId);
            var courseAssignedStudent = await qalreadyAssingedStudent.LastOrDefaultAsync();
            if (!courseAssignedStudentId.HasValue && courseAssignedStudent != null && (courseAssignedStudent.Status == AssignedStatus.Accepted || courseAssignedStudent.Status == AssignedStatus.Completed))
            {
                courseAssignedStudentId = courseAssignedStudent.Id;
            }

            var courseId = (await _ws.GetRepo<CourseInstance>().GetAsync(courseInstanceId)).CourseId;
            var totalPage = await _ws.GetAll<Page>().CountAsync(p => p.CourseId == courseId && p.Type == PageType.Page);
            var nCompletedPage = 0;
            if (courseAssignedStudentId.HasValue)
            {
                nCompletedPage = await _ws.GetRepo<StudentProgress>().GetAllIncluding(s => s.Page)
                .CountAsync(sp => sp.Progress == StudentProgressStatus.Completed && sp.CourseInstanceId == courseInstanceId && sp.CourseAssignedStudentId == courseAssignedStudent.Id && sp.Page.Type == PageType.Page);
            }


            var now = DateTime.UtcNow;

            // get students are assigned via group
            var qassignedGroupIds =
                from g in _ws.GetAll<GroupAssignedCourse>().Where(s => s.CourseInstanceId == courseInstanceId)
                select new { g.GroupId, g.CourseInstanceId };

            // ignore user already assigned to system
            var qassignStudentIds =
                from ug in _ws.GetAll<UserGroup>()
                join g in qassignedGroupIds on ug.GroupId equals g.GroupId into groups
                where groups.Any()
                select ug.UserId;

            var isCompletedCourse = (courseAssignedStudent != null && courseAssignedStudent.Status == AssignedStatus.Completed);


            //var qCourse = _ws.GetAll<Course>().Include(s => s.Language);
            var qCourse = _ws.GetRepo<Course>().GetAllIncluding(s => s.Language).Where(c => c.Id == courseId);
            var qCourseInstance = _ws.GetAll<CourseInstance>().Where(s => s.Id == courseInstanceId);
            var query = from c in qCourse
                        join ci in qCourseInstance on c.Id equals ci.CourseId
                        join ct in
                            from ct in _ws.GetAll<CourseTag>()
                            join ca in _ws.GetAll<Category>() on ct.CategoryId equals ca.Id
                            select new { ct.CourseId, ca.Name }
                        on c.Id equals ct.CourseId into courseTags
                        join assignedStudent in qalreadyAssingedStudent on ci.Id equals assignedStudent.CourseInstanceId into userAssignStatus
                        from status in userAssignStatus.OrderByDescending(s => s.CreationTime).DefaultIfEmpty().Take(1)
                        join gci in
                            from ga in _ws.GetAll<GroupAssignedCourse>()
                            join ug in _ws.GetAll<UserGroup>().Where(s => s.UserId == AbpSession.UserId.Value) on ga.GroupId equals ug.GroupId
                            select ga.CourseInstanceId
                        on ci.Id equals gci into groups
                        join cl in _ws.GetAll<CourseLevel>() on c.LevelId.Value equals cl.Id into courseLevel
                        from cl in courseLevel.DefaultIfEmpty().Take(1)
                        join uex in
                             from userrole in _ws.GetAll<UserExtraRole>()
                             join role in _ws.GetAll<Role, int>().Where(s => s.Name == StaticRoleNames.Tenants.CourseAdmin) on userrole.RoleId equals role.Id
                             join u in _ws.GetAll<User, long>() on userrole.UserId equals u.Id
                             select new { userrole.EntityId, User = u }
                        on c.Id equals uex.EntityId into Instructors
                        join creator in _ws.GetAll<User, long>().Include(s => s.Language) on c.CreatorUserId equals creator.Id
                        join s in _ws.GetAll<LMSSetting>().Where(s => s.EntityType == nameof(Course) && s.Name == Const.Allow_completed_students_to_re_enroll) on c.Id equals s.EntityId into lmsSettings
                        from lmsSetting in lmsSettings.DefaultIfEmpty().Take(1)

                        select new
                        {
                            Course = new { c.Id, c.Name, c.Description, c.RelatedInformation, c.Syllabus, c.LanguageId, c.ImageCover, c.Sourse, c.SoursePath },
                            ci.StartTime,
                            ci.EndTime,
                            Length = ci.EndTime == null || ci.StartTime == null ? 0 : Math.Round((ci.EndTime.Value - ci.StartTime.Value).TotalDays / 7),
                            Status = status != null ? status.Status : (AssignedStatus?)null,
                            CategoryName = courseTags.Select(s => s.Name),
                            InvitedInGroup = groups.Any(),
                            Level = cl.DisplayName,
                            Instructors = Instructors.Select(s => new { s.User.Id, s.User.FullName, s.User.Avatar, s.User.UserName }),
                            CourseLanguage = c.Language != null ? c.Language.DisplayName : null,
                            CreatorLanguage = creator.Language != null ? creator.Language.DisplayName : null,
                            IsSelfPaced = !ci.NumberDayToStudy.HasValue,
                            NPageCompleted = nCompletedPage,//for test
                            TotalPage = totalPage,
                            isArchived = ci.EndTime != null && ci.EndTime.Value < now,
                            CanStart = !((c.RestrictStudentFromViewThisCourseAfterEndDate && ci.EndTime != null && ci.EndTime.Value < now)
                                        || (c.RestrictStudentsFromViewingThisCourseBeforeEndDate && ci.StartTime != null && ci.StartTime.Value > now)),
                            Creator = new { creator.Id, creator.FullName, creator.Avatar, creator.UserName },
                            CanReEnroll = isCompletedCourse && c.Type == CourseType.Recur && (ci.EndTime == null || now < ci.EndTime) && lmsSetting != null && Boolean.Parse(lmsSetting.Value)

                        };

            return await query.FirstOrDefaultAsync();
        }
        #endregion

        public async Task<EditCourseSyllabusInput> UpdateCourseSyllabus(EditCourseSyllabusInput input)
        {
            CheckUpdatePermission();
            var item = await _ws.GetRepo<Course>().GetAsync(input.Id);
            item.Syllabus = input.Syllabus;
            await _ws.UpdateAsync(item);
            return input;
        }
        #region CourseLMSSetting
        public async Task<IEnumerable<LMSSettingInput>> CreateOrUpdateCourseLMSSetting(IEnumerable<LMSSettingInput> inputs)
        {
            foreach (var input in inputs)
            {
                var item = await _ws.GetRepo<LMSSetting>().GetAll().FirstOrDefaultAsync(m => m.Name == input.Key && m.EntityId == input.EntityId);

                if (item == null) // Insert
                {
                    //var course = await _ws.GetRepo<Course>().GetAsync(input.EntityId);
                    item = new LMSSetting()
                    {
                        EntityId = input.EntityId,
                        Name = input.Key,
                        //TenantId = course.TenantId,
                        Value = input.Value.ToString(),
                        EntityType = nameof(Course),
                    };
                    await _ws.InsertAsync(item);
                }
                else // Update
                {
                    item.Value = input.Value.ToString();
                    await _ws.UpdateAsync(item);
                }
            }
            return inputs;
        }
        public async Task<ListResultDto<LMSSettingOut>> GetCourseLMSSettingValue(Guid courseId)
        {
            var qLMSSettingValue = _ws.GetRepo<LMSSetting>().GetAll()
                               .Where(m => m.EntityType == nameof(Course) && m.EntityId == courseId)
                               .Select(m => new LMSSettingOut
                               {
                                   Key = m.Name,
                                   Value = m.Value,
                               });
            var result = await qLMSSettingValue.ToListAsync();
            return new ListResultDto<LMSSettingOut>(result);
        }
        #endregion

        #region dashboard for course admin
        // /// <summary>
        // /// Get All Course For system-admin page and course-admin page
        // /// </summary>
        // /// <returns></returns>
        public async Task<List<CourseDashboardDto>> GetAllCourseNotPagging()
        {
            var Now = DateTime.UtcNow;
            var ComminSoonDate = DateTime.UtcNow.AddDays(14);
            bool isSystemAdmin = _userService.UserHasRole(AbpSession.UserId.Value, StaticRoleNames.Tenants.Admin);
            var roleCourseAdminId = await _ws.GetAll<Role, int>().Where(r => r.Name == StaticRoleNames.Tenants.CourseAdmin).Select(r => r.Id).FirstOrDefaultAsync();

            if (isSystemAdmin)
            {
                var qcourseDashboard =
               (from ci in _ws.GetAll<CourseInstance>()
                join c in _ws.GetAll<Course>() on ci.CourseId equals c.Id
                where ci.Status == CourseSettingStatus.Active

                let AlreadyStart = ci.StartTime <= Now && ci.EndTime >= Now
                let IsCommingSoon = ci.StartTime <= ComminSoonDate && ci.StartTime > Now
                let IsUpComming = ci.StartTime.HasValue && ci.StartTime.Value > ComminSoonDate


                select new CourseDashboardDto
                {
                    Id = ci.Id,
                    StartDate = ci.StartTime,
                    EndDate = ci.EndTime,
                    Name = c.Name,
                    Description = c.Description,
                    RelationInfo = c.RelatedInformation,
                    CurrentPoint = AlreadyStart ? 100 : 0,
                    ComminSoonDatePoint = IsCommingSoon ? 10 : 0,
                    AlreadyStart = AlreadyStart,
                    IsCommingSoon = IsCommingSoon,
                    IsUpComming = IsUpComming,
                    IsSelfPaced = !ci.NumberDayToStudy.HasValue,
                    ImageCover = c.ImageCover,
                    State = c.State,
                    IsOwner = c.CreatorUserId == AbpSession.UserId.Value,
                    IsArchived = (ci.EndTime ?? DateTime.MaxValue) < DateTimeUtils.GetNow()
                }).OrderBy(s => s.CurrentPoint + s.ComminSoonDatePoint).ThenByDescending(s => s.StartDate);
                return await qcourseDashboard.ToListAsync<CourseDashboardDto>();
            }
            else
            {
                var qcourseDashboard =
               (from ci in _ws.GetAll<CourseInstance>()
                join c in _ws.GetAll<Course>() on ci.CourseId equals c.Id
                join uer in _ws.GetAll<UserExtraRole>().Where(u => u.EntityType == nameof(Course) && u.RoleId == roleCourseAdminId) on c.Id equals uer.EntityId into users
                where ci.Status == CourseSettingStatus.Active && (ci.CreatorUserId == AbpSession.UserId || users.Select(u => u.UserId).Contains(AbpSession.UserId.Value))

                let AlreadyStart = ci.StartTime <= Now && ci.EndTime >= Now
                let IsCommingSoon = ci.StartTime <= ComminSoonDate && ci.StartTime > Now
                let IsUpComming = ci.StartTime.HasValue && ci.StartTime.Value > ComminSoonDate


                select new CourseDashboardDto
                {
                    Id = ci.Id,
                    StartDate = ci.StartTime,
                    EndDate = ci.EndTime,
                    Name = c.Name,
                    Description = c.Description,
                    RelationInfo = c.RelatedInformation,
                    CurrentPoint = AlreadyStart ? 100 : 0,
                    ComminSoonDatePoint = IsCommingSoon ? 10 : 0,
                    AlreadyStart = AlreadyStart,
                    IsCommingSoon = IsCommingSoon,
                    IsUpComming = IsUpComming,
                    IsSelfPaced = !ci.NumberDayToStudy.HasValue,
                    ImageCover = c.ImageCover,
                    State = c.State,
                    IsOwner = c.CreatorUserId == AbpSession.UserId.Value
                }).OrderBy(s => s.CurrentPoint + s.ComminSoonDatePoint).ThenByDescending(s => s.StartDate);
                return await qcourseDashboard.ToListAsync<CourseDashboardDto>();
            }

        }

        public async Task<PagedResultDto<CourseFAQDto>> GetAllCourseFAQ()
        {
            var currentId = AbpSession.UserId;
            if (!currentId.HasValue)
            {
                return null;
            }
            bool isSystemAdmin = _userService.UserHasRole(AbpSession.UserId.Value, StaticRoleNames.Tenants.Admin);

            var qTeacherViewDiscussion = _ws.GetRepo<TeacherViewDiscussion, Guid>().GetAll().Where(m => m.CreatorUserId == currentId.Value).Select(x => x.QAId);

            var qQuestionResponse = from aw in _ws.GetRepo<QAAnswer, Guid>().GetAll().Where(m => m.ResponseParentId == null)
                                    join qs in _ws.GetRepo<QAQuestion, Guid>().GetAll()
                                    on aw.QuestionId equals qs.Id
                                    select new
                                    {
                                        AnswerId = aw.Id,
                                        CourseInstanceId = qs.CourseInstanceId,
                                        CreaterUserId = aw.CreatorUserId,
                                    };

            var qCourse = from courseIns in _ws.GetRepo<CourseInstance, Guid>().GetAll().Where(m => m.Status == CourseSettingStatus.Active)
                          join course in _ws.GetRepo<Course, Guid>().GetAll()
                          on courseIns.CourseId equals course.Id
                          let faqs = _ws.GetRepo<FAQQuestion, Guid>().GetAll().Where(x => x.CourseId == course.Id)
                          let qaQs = _ws.GetRepo<QAQuestion, Guid>().GetAll().Where(x => x.CourseInstanceId == courseIns.Id)

                          let qaRs = qQuestionResponse.Where(x => x.CourseInstanceId == courseIns.Id)

                          select new CourseFAQDto
                          {
                              Id = course.Id,
                              CourseInstanceId = courseIns.Id,
                              Name = course.Name,
                              ImageCover = course.ImageCover,
                              StartTime = courseIns.StartTime,
                              EndTime = courseIns.EndTime,

                              TotalFAQ = faqs.Count(),
                              TotalQuestion = qaQs.Count(),
                              TotalResponse = qaRs.Count(),

                              IsReadedQuestion = (qaQs.Where(m => m.CreatorUserId != currentId).Select(p => p.Id).Except(qTeacherViewDiscussion)).Count() > 0 ? true : false,
                              IsReadedResponse = (qaRs.Where(m => m.CreaterUserId != currentId).Select(p => p.AnswerId).Except(qTeacherViewDiscussion)).Count() > 0 ? true : false,

                              State = (courseIns.EndTime ?? DateTime.MaxValue) < DateTimeUtils.GetNow() ? CourseState.Archived : course.State,
                              CreationTime = course.CreationTime,
                              CreatorUserId = courseIns.CreatorUserId,
                          };
            if (!isSystemAdmin)
            {
                var qCourseAssign = from course in qCourse
                                    join userExtraRole in _ws.GetRepo<UserExtraRole, Guid>().GetAll().Where(m => m.UserId == currentId && m.EntityType == nameof(Course))
                                    on course.Id equals userExtraRole.EntityId
                                    select course;
                qCourse = qCourse.Where(m => m.CreatorUserId == currentId).Concat(qCourseAssign);
            }

            var result = await qCourse.OrderByDescending(m => m.CreationTime).ToListAsync();
            return new PagedResultDto<CourseFAQDto>(result.Count(), result);

        }

        #endregion


        #region student - courses 
        [AbpAuthorize(Authorization.PermissionNames.Pages_CourseView)]
        public async Task<List<StudentCourseDto>> GetAcceptedCourses()
        {
            var courseProgress = _ws.GetRepo<StudentProgress>().GetAllIncluding(s => s.Page)
                                 .Where(s => s.CreatorUserId == AbpSession.UserId.Value && s.Progress == StudentProgressStatus.Completed && s.Page.Type == PageType.Page)
                                 .GroupBy(s => s.CourseInstanceId)
                                 .Select(s => new { CourseInstanceId = s.Key, Progress = s.Count() });
            var qtotalPages = _ws.GetAll<Page>().Where(s => s.Type == PageType.Page).
                GroupBy(s => s.CourseId)
                .Select(s => new { CourseId = s.Key, TotalPage = s.Count() });

            var query = from cas in _ws.GetAll<CourseAssignedStudent>().Where(s => s.StudentId == AbpSession.UserId && s.Status == AssignedStatus.Accepted)
                        join ci in _ws.GetAll<CourseInstance>().Where(cii => !cii.EndTime.HasValue || cii.EndTime.Value >= DateTime.UtcNow) on cas.CourseInstanceId equals ci.Id
                        join c in _ws.GetAll<Course>().Where(c => c.State == CourseState.Publish) on ci.CourseId equals c.Id
                        join tp in qtotalPages on c.Id equals tp.CourseId into pages
                        join cp in courseProgress on cas.CourseInstanceId equals cp.CourseInstanceId into progresses
                        join cl in _ws.GetAll<CourseColor>().Where(s => s.CreatorUserId == AbpSession.UserId) on c.Id equals cl.CourseId into courseColor
                        from progress in progresses.DefaultIfEmpty().Take(1)
                        from page in pages.DefaultIfEmpty().Take(1)
                        from clr in courseColor.DefaultIfEmpty().Take(1)
                        select new StudentCourseDto
                        {
                            CourseId = c.Id,
                            CourseInstanceId = ci.Id,
                            Name = c.Name,
                            ColorCode = clr != null ? clr.ColorCode : null,
                            ImageCover = c.ImageCover,
                            RelatedInformation = c.RelatedInformation,
                            State = cas.Status,
                            StartTime = ci.StartTime,
                            EndTime = ci.EndTime,
                            NCompletedPage = progress != null ? progress.Progress : 0,
                            TotalPage = page == null ? 0 : page.TotalPage,
                        };
            return await query.ToListAsync();
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_CourseView)]
        public async Task<List<StudentCourseDto>> GetCompletedCourses()
        {
            var courseProgress = _ws.GetRepo<StudentProgress>().GetAllIncluding(s => s.Page)
                                 .Where(s => s.CreatorUserId == AbpSession.UserId.Value && s.Progress == StudentProgressStatus.Completed && s.Page.Type == PageType.Page)
                                 .GroupBy(s => s.CourseInstanceId)
                                 .Select(s => new { CourseInstanceId = s.Key, Progress = s.Count() });
            var qtotalPages = _ws.GetAll<Page>().Where(s => s.Type == PageType.Page).
                GroupBy(s => s.CourseId)
                .Select(s => new { CourseId = s.Key, TotalPage = s.Count() });

            var query = from cas in _ws.GetAll<CourseAssignedStudent>().Where(s => s.StudentId == AbpSession.UserId && s.Status == AssignedStatus.Completed)
                        join ci in _ws.GetAll<CourseInstance>().Where(cii => !cii.EndTime.HasValue || cii.EndTime.Value >= DateTime.UtcNow) on cas.CourseInstanceId equals ci.Id
                        join c in _ws.GetAll<Course>().Where(c => c.State == CourseState.Publish) on ci.CourseId equals c.Id
                        join tp in qtotalPages on c.Id equals tp.CourseId into pages
                        join cp in courseProgress on cas.CourseInstanceId equals cp.CourseInstanceId into progresses
                        from progress in progresses.DefaultIfEmpty().Take(1)
                        from page in pages.DefaultIfEmpty().Take(1)
                        select new StudentCourseDto
                        {
                            CourseId = c.Id,
                            CourseInstanceId = ci.Id,
                            Name = c.Name,
                            ImageCover = c.ImageCover,
                            RelatedInformation = c.RelatedInformation,
                            State = cas.Status,
                            StartTime = ci.StartTime,
                            EndTime = ci.EndTime,
                            NCompletedPage = progress != null ? progress.Progress : 0,
                            TotalPage = page == null ? 0 : page.TotalPage,
                        };
            return await query.ToListAsync();
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_Calendar)]
        public async Task<List<StudentCourseDto>> GetAcceptedCoursesForCalendar()
        {

            var query = from cas in _ws.GetAll<CourseAssignedStudent>().Where(s => s.StudentId == AbpSession.UserId && s.Status == AssignedStatus.Accepted)
                        join ci in _ws.GetAll<CourseInstance>().Where(cii => !cii.EndTime.HasValue || cii.EndTime.Value >= DateTime.UtcNow) on cas.CourseInstanceId equals ci.Id
                        join c in _ws.GetAll<Course>().Where(c => c.State == CourseState.Publish) on ci.CourseId equals c.Id
                        join cl in _ws.GetAll<CourseColor>().Where(s => s.CreatorUserId == AbpSession.UserId) on c.Id equals cl.CourseId into courseColor
                        from clr in courseColor.DefaultIfEmpty().Take(1)
                        select new StudentCourseDto
                        {
                            CourseId = c.Id,
                            CourseInstanceId = ci.Id,
                            Name = c.Name,
                            ColorCode = clr != null ? clr.ColorCode : null,
                            StartTime = ci.StartTime,
                            EndTime = ci.EndTime,
                        };
            return await query.ToListAsync();
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_CourseView)]
        public async Task<List<StudentCourseDto>> GetArchivedCourses()
        {
            var query = _courseManager.GetAvailableCoursesForStudent(AbpSession.UserId.Value).Where(s => s.CourseInstance.EndTime.HasValue && s.CourseInstance.EndTime.Value < DateTime.UtcNow)
                .Select(s => new StudentCourseDto
                {
                    CourseId = s.CourseInstance.CourseId,
                    CourseInstanceId = s.CourseInstance.Id,
                    Name = s.CourseInstance.Course.Name,
                    ImageCover = s.CourseInstance.Course.ImageCover,
                    RelatedInformation = s.CourseInstance.Course.RelatedInformation,
                    //CompletedPercent = 30,//for test

                });

            return await query.ToListAsync();
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_CourseView)]
        public async Task<List<StudentCourseDto>> GetInvitedCourses()
        {
            var query = _courseManager.GetAvailableCoursesForStudent(AbpSession.UserId.Value)
                .Where(s => (!s.CourseInstance.EndTime.HasValue || s.CourseInstance.EndTime.Value >= DateTime.UtcNow) && (!s.AssignedStatus.HasValue || s.AssignedStatus == AssignedStatus.Invited))
                .Select(s => new StudentCourseDto
                {
                    CourseId = s.CourseInstance.CourseId,
                    CourseInstanceId = s.CourseInstance.Id,
                    Name = s.CourseInstance.Course.Name,
                    ImageCover = s.CourseInstance.Course.ImageCover,
                    RelatedInformation = s.CourseInstance.Course.RelatedInformation,
                    //CompletedPercent = 30,//for test

                });

            return await query.ToListAsync();
        }


        #endregion

        #region CourseLevel CRUD
        [AbpAuthorize(Authorization.PermissionNames.Pages_Settings)]
        public async Task<List<CourseLevelDto>> GetAllCourseLevel()
        {
            var currentTennat = AbpSession.TenantId.Value;
            var qCourseLevel = await _ws.GetAll<CourseLevel>().Where(c => c.TenantId == currentTennat).OrderBy(m => m.Level).ToListAsync();
            var lstCourseLevel = ObjectMapper.Map<List<CourseLevelDto>>(qCourseLevel);
            return lstCourseLevel;
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_Settings)]
        public async Task<CourseLevelDto> CreateCourseLevel(CourseLevelDto input)
        {
            var exists = await _ws.GetAll<Entities.CourseLevel>().AnyAsync(s => s.Level == input.Level || s.DisplayName == input.DisplayName);
            if (exists)
            {
                throw new UserFriendlyException($"Level \"{input.Level}\"  or Name \"{ input.DisplayName }\" existed");
            }
            var courseLevel = ObjectMapper.Map<CourseLevel>(input);
            input.Id = await _ws.InsertAndGetIdAsync(courseLevel);
            return input;
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_Settings)]
        public async Task<CourseLevelDto> UpdateCourseLevel(CourseLevelDto input)
        {
            var exists = await _ws.GetAll<Entities.CourseLevel>().AnyAsync(s => s.Id != input.Id && (s.Level == input.Level || s.DisplayName == input.DisplayName));
            if (exists)
            {
                throw new UserFriendlyException($"Level \"{input.Level}\"  or Name \"{ input.DisplayName }\" existed");
            }
            var qCourseLevel = await _ws.GetRepo<CourseLevel>().GetAsync(input.Id);
            ObjectMapper.Map<CourseLevelDto, CourseLevel>(input, qCourseLevel);
            await _ws.UpdateAsync(qCourseLevel);
            return input;
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_Settings)]
        public async Task DeleteCourseLevel(EntityDto<Guid> input)
        {
            await _ws.GetRepo<CourseLevel>().DeleteAsync(input.Id);
        }
        [AbpAuthorize(Authorization.PermissionNames.Pages_Settings)]
        public async Task<int> CheckDeleteCourseLevel(EntityDto<Guid> input)
        {
            return await _ws.GetAll<Course, Guid>().Where(m => m.LevelId == input.Id).CountAsync();
        }
        #endregion
        #region Course Color CRUD
        public async Task<CourseColorDto> CreateOrUpdateCourseColor(CourseColorDto input)
        {
            var currentUserId = AbpSession.UserId.Value;
            var query = _ws.GetAll<CourseColor>().Any(c => c.CourseId == input.CourseId && c.CreatorUserId == currentUserId);
            if (query)
            {
                var qCourseColor = await _ws.GetAll<CourseColor>().Where(c => c.CourseId == input.CourseId && c.CreatorUserId == currentUserId).FirstOrDefaultAsync();
                ObjectMapper.Map<CourseColorDto, CourseColor>(input, qCourseColor);
                await _ws.UpdateAsync(qCourseColor);
            }
            else
            {
                var courseColor = ObjectMapper.Map<CourseColor>(input);
                await _ws.InsertAsync(courseColor);
            }
            return input;
        }
        #endregion


        #region Course-statistics
        [HttpPost]
        [AbpAuthorize(Authorization.PermissionNames.Pages_Courses)]
        public async Task<CourseStatisticsDto> GetCourseStatistics(StatisticGridParam Input)
        {
            var courseInstanceId = Input.courseInstanceId;

            var course = await _ws.GetRepo<CourseInstance>().GetAsync(courseInstanceId);
            var courseId = course.CourseId;
            var item = new CourseStatisticsDto();

            item.TotalPage = await _ws.GetAll<Page>().CountAsync(s => s.CourseId == courseId && s.Type == PageType.Page);

            //var qassignedstudents = _courseManager.GetStudentAssignedCourseByStatus(courseInstanceId, AssignedStatus.Accepted);
            var qassignedstudents = _courseManager.GetCourseAssignedStudentsAcceptedAndCompleted(courseInstanceId);
            var query = from ast in qassignedstudents
                        join u in _ws.GetAll<User, long>() on ast.StudentId equals u.Id
                        join stp in _ws.GetRepo<StudentProgress>().GetAllIncluding(s => s.Page).Where(s => s.Page.CourseId == courseId && s.Progress == StudentProgressStatus.Completed && s.Page.Type == PageType.Page)
                        on ast.Id equals stp.CourseAssignedStudentId into completedPages
                        join stta in
                             from ta in _ws.GetRepo<TestAttempt>().GetAllIncluding(a => a.CourseAssignedStudent)
                             join q in _ws.GetRepo<QuizSetting>().GetAllIncluding(q => q.Quiz).Where(qs => qs.CourseInstanceId == courseInstanceId && qs.Quiz.Type == QuizType.Survey)
                             on ta.QuizSettingId equals q.Id
                             select ta
                        on u.Id equals stta.CourseAssignedStudent.StudentId into studentsurveys
                        select new StudentDto
                        {
                            CourseAssignedStudentId = ast.Id,
                            StudentId = u.Id,
                            Name = u.UserName,
                            IsDoneSurvey = studentsurveys.Count() > 0,
                            NCompletedPage = completedPages.Count(s => s.CourseAssignedStudentId == ast.Id),
                            TotalScore = 0,
                            CourseAssignedStudentTime = ast.CreationTime,
                            Status = ast.Status,
                            EnrollCount = ast.EnrollCount,
                            Score = 0
                        };
            item.Students = await query.OrderByDescending(x=> x.StudentId).GetGridResult(query,Input);

            //get quizes
            item.Quizzes = await _ws.GetRepo<QuizSetting>().GetAllIncluding(s => s.Quiz).Where(s => s.CourseInstanceId == courseInstanceId && s.Quiz.Type == QuizType.Quiz)
                .Select(s => new SQuizDto { Id = s.Id, Name = s.Quiz.Title, Score = s.Point.HasValue ? s.Point.Value : 0, ScoreToKeepType = s.Quiz.ScoreKeepType, QuizType = s.Quiz.Type }).ToListAsync();

            //get assignments
            item.Assignments = await _ws.GetRepo<AssignmentSetting>().GetAllIncluding(s => s.Assignment).Where(s => s.CourseInstanceId == courseInstanceId)
                .Select(s => new SAssignmentDto
                {
                    Id = s.Id,
                    Name = s.Assignment.Title,
                    Score = s.Point.HasValue ? s.Point.Value : 0,
                    IsAssignIndividualGrade = s.Assignment.IsAssignIndividualGrade,
                    DisplayGrade = s.Assignment.DisplayGrade,
                    IsGroupAssignment = s.Assignment.IsGroupAssignment
                }).ToListAsync();

            #region student quiz score           

            var studentAssignedQuiz = _courseManager.GetStudentAssignedQuizzes(courseInstanceId).ToList();

            //student quiz score
            var studentQuizScores = await (from student in qassignedstudents
                                           join ta in _ws.GetRepo<TestAttempt>().GetAllIncluding(s => s.QuizSetting, s => s.QuizSetting.Quiz).Where(s => s.QuizSetting.CourseInstanceId == courseInstanceId && s.Status == TestAttemptStatus.Marking && s.QuizSetting.Quiz.Type == QuizType.Quiz)
                                           on student.Id equals ta.CourseAssignedStudentId
                                           select new
                                           {
                                               ta.CourseAssignedStudentId,
                                               student.StudentId,
                                               ta.Score,
                                               ta.MaxScore,
                                               ta.QuizSettingId,
                                               ta.QuizSetting.Quiz.ScoreKeepType
                                           }).ToListAsync();

            var studentCount = item.Students.Items.Count();
            var quizCount = item.Quizzes.Count();
            item.StudentQuizScores = new float?[studentCount, quizCount];
            for (int i = 0; i < studentCount; i++)
            {
                item.Students.Items[i].Score = 0;
                item.Students.Items[i].TotalScore = 0;
                var quizes = studentAssignedQuiz.Where(s => s.CourseAssignedStudentId == item.Students.Items[i].CourseAssignedStudentId).FirstOrDefault().AssignedList;
                for (int j = 0; j < quizCount; j++)
                {
                    var quiz = quizes.Where(s => s.SettingId == item.Quizzes[j].Id).FirstOrDefault();
                    if (quiz == null)
                    {
                        //quiz is not assign to student
                        item.StudentQuizScores[i, j] = null;
                    }
                    else
                    {
                        item.StudentQuizScores[i, j] = 0;
                        if (item.Quizzes[j].ScoreToKeepType == QuizScoreToKeepType.Highest)
                        {
                            var score = studentQuizScores.Where(s => s.CourseAssignedStudentId == item.Students.Items[i].CourseAssignedStudentId && s.QuizSettingId == item.Quizzes[j].Id)
                                .OrderByDescending(s => s.MaxScore.HasValue && s.MaxScore.Value > 0 ? s.Score / s.MaxScore : 0).FirstOrDefault();
                            if (score != null && score.Score.HasValue && score.MaxScore.HasValue)
                            {
                                item.StudentQuizScores[i, j] = score.Score / score.MaxScore * quiz.Point;
                            }

                        }
                        else
                        {
                            var scores = studentQuizScores.Where(s => s.CourseAssignedStudentId == item.Students.Items[i].CourseAssignedStudentId && s.QuizSettingId == item.Quizzes[j].Id).ToList();
                            if (scores != null && scores.Count() > 0)
                            {
                                var score = scores.Average(s => s.MaxScore.HasValue && s.MaxScore.Value > 0 ? (s.Score ?? 0) / s.MaxScore : 0);
                                item.StudentQuizScores[i, j] = score.HasValue ? score * quiz.Point : 0;
                            }

                        }
                        item.Students.Items[i].Score += item.StudentQuizScores[i, j] ?? 0;
                        item.Students.Items[i].TotalScore += quiz.Point.Value;
                    }
                }
            }

            #endregion student quiz score


            #region student assignment score         
            var studentAssignedAssignment = _courseManager.GetStudentAssignedAssignments(courseInstanceId);
            //student assignment score
            var studentAssignmentScores = await (from student in qassignedstudents
                                                 join sa in _ws.GetRepo<StudentAssignment>().GetAllIncluding(s => s.Assignment).Where(s => s.Assignment.CourseInstanceId == courseInstanceId)
                                                 on student.Id equals sa.CourseAssignedStudentId
                                                 select new StudentAssignmentDto
                                                 {
                                                     Id = sa.Id,
                                                     AssignmentSettingId = sa.AssignmentSettingId,
                                                     CourseAssignedStudentId = sa.CourseAssignedStudentId,
                                                     Point = sa.Point
                                                 }
                                           ).ToListAsync();

            item.StudentAssignments = studentAssignmentScores;

            var assignmentCount = item.Assignments.Count();
            item.StudentAssignmentScores = new float?[studentCount, assignmentCount];
            for (int i = 0; i < studentCount; i++)
            {
                var assignments = studentAssignedAssignment.Where(s => s.CourseAssignedStudentId == item.Students.Items[i].CourseAssignedStudentId).FirstOrDefault().AssignedList;
                for (int j = 0; j < assignmentCount; j++)
                {
                    var assignment = assignments.Where(s => s.SettingId == item.Assignments[j].Id).FirstOrDefault();
                    if (assignment == null)
                    {
                        //assignment is not assign to student
                        item.StudentAssignmentScores[i, j] = null;
                    }
                    else
                    {
                        var score = studentAssignmentScores.Where(s => s.CourseAssignedStudentId == item.Students.Items[i].CourseAssignedStudentId && s.AssignmentSettingId == item.Assignments[j].Id)
                            .FirstOrDefault();
                        item.StudentAssignmentScores[i, j] = score == null ? 0 : score.Point;
                        item.Students.Items[i].Score += item.StudentAssignmentScores[i, j].Value;
                        item.Students.Items[i].TotalScore += assignment.Point.Value;
                    }
                }
            }

            #endregion student assignment score

            return item;
        }

        //public Task<List<StudentQuizAssignmentScore>> GetStudentQuizScores(Guid courseInstanceId, long studentId)
        //{
        //    return _courseManager.GetStudentQuizScores(studentId, courseInstanceId);
        //}
        #endregion

        [AbpAuthorize(Authorization.PermissionNames.Pages_Courses)]
        public async Task<CourseInstance> RePublishCourse(Guid courseInstanceId)
        {
            var courseInstance = await _courseManager.RePublishCourse(courseInstanceId);
            CurrentUnitOfWork.SaveChanges();
            return courseInstance;
        }

        public async Task<List<CourseInstanceExpiredDto>> getCourseInstancesHistory(Guid courseInstanceId)
        {
            var courseInstance = _ws.GetRepo<CourseInstance>().Get(courseInstanceId);
            var course = _ws.GetRepo<Course>().Get(courseInstance.CourseId);
            var qcourseDashboard =
           (from ci in _ws.GetAll<CourseInstance>()
            join u in _ws.GetAll<User, long>() on ci.CreatorUserId equals u.Id
            where ci.Status == CourseSettingStatus.Deactive && ci.CourseId == course.Id
            select new CourseInstanceExpiredDto
            {
                Id = ci.Id,
                StartTime = ci.StartTime,
                EndTime = ci.EndTime,
                CreaterName = u.FullName,

            }).OrderBy(s => s.EndTime);
            return await qcourseDashboard.ToListAsync();
        }
        public async Task DeleteCourse(EntityDto<Guid> input)
        {
            var courseInstance = _ws.GetRepo<CourseInstance, Guid>().GetAllIncluding(ci => ci.Course).Where(ci => ci.Id == input.Id).FirstOrDefault();
            await _ws.GetRepo<Course>().DeleteAsync(courseInstance.Course);
        }
        [HttpGet]
        public async Task<CourseDashboardDto> CheckCourseInfo(EntityDto<Guid> input)
        {
            var result = new CourseDashboardDto();
            var courseInstance = _ws.GetRepo<CourseInstance, Guid>().GetAllIncluding(ci => ci.Course).Where(ci => ci.Id == input.Id).FirstOrDefault();
            if (courseInstance != null)
            {
                result.Id = courseInstance.Id;
                var assignStudents = await (from ci in _ws.GetAll<CourseInstance>().Where(cis => cis.CourseId == courseInstance.Course.Id)
                                            join cg in _ws.GetAll<CourseAssignedStudent>().Where(cas => cas.Status == AssignedStatus.Completed || cas.Status == AssignedStatus.Accepted)
                                            on ci.Id equals cg.CourseInstanceId
                                            select cg).ToListAsync();
                result.AlreadyStart = assignStudents.Count() > 0;
            }
            return result;
        }
        [HttpGet]
        public async Task<SQuizDto> GetStudentSurvey(EntityDto<Guid> input)
        {
            var surveyQuiz = await _ws.GetRepo<TestAttempt>().GetAllIncluding(a => a.QuizSetting, a => a.QuizSetting.Quiz).Where(a => a.CourseAssignedStudentId == input.Id && a.QuizSetting.Quiz.Type == QuizType.Survey)
                .Select(s => new SQuizDto { Id = s.QuizSetting.Id, Name = s.QuizSetting.Quiz.Title, ScoreToKeepType = s.QuizSetting.Quiz.ScoreKeepType, QuizType = s.QuizSetting.Quiz.Type }).FirstOrDefaultAsync();
            if (surveyQuiz != null)
                return surveyQuiz;
            else
                return null;
        }
    }
}

