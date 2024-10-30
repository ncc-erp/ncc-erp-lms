using System.ComponentModel.DataAnnotations;

namespace RMALMS.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}