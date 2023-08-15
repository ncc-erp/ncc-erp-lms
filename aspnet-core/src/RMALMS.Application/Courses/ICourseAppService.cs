using Abp.Application.Services;
using Abp.Application.Services.Dto;
using RMALMS.Courses.Dto;
using RMALMS.Entities;
using RMALMS.Paging;
using RMALMS.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.Courses
{
    public interface ICourseAppService: IAsyncCrudAppService<EditCourseDto, Guid, PagedResultRequestDto, CreateCourseDto, EditCourseDto>
    {
        ListResultDto<PermissionDto> GetAllPermissions();        
        Task<GridResult<CourseDto>> GetAllPagging(GridParam input);

        #region Course People
        Task<GridResult<AssignedStudentCourseDto>> GetAssignedStudentByCourseAndStatus(RequestAssignedStudentCourseDto requestDto);
        Task<GridResult<AssignedStudentCourseDto>> GetInvitationStudentByCourse(InvitationCourseRequestDto requestDto);
        #endregion
        Task<EditCourseSyllabusInput> UpdateCourseSyllabus(EditCourseSyllabusInput input);
        #region CourseLMSSetting
        Task<IEnumerable<LMSSettingInput>> CreateOrUpdateCourseLMSSetting(IEnumerable<LMSSettingInput> input);
        Task<ListResultDto<LMSSettingOut>> GetCourseLMSSettingValue(Guid courseId);
        #endregion
        #region CourseLevel
        Task<List<CourseLevelDto>> GetAllCourseLevel();
        Task<CourseLevelDto> CreateCourseLevel(CourseLevelDto input);
        Task<CourseLevelDto> UpdateCourseLevel(CourseLevelDto input);
        Task DeleteCourseLevel(EntityDto<Guid> input);
        #endregion

    }
}
