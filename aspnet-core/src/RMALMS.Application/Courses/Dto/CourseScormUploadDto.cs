using Microsoft.AspNetCore.Http;
using RMALMS.Entities;

namespace RMALMS.Courses.Dto
{
    public class CourseScormUploadDto
    {
        public string Title { get; set; }
        public CourseSource SourseVersion { get; set; }
    }
    public class FileSCORMInput
    {
        public IFormFile File { get; set; }
    }
}
