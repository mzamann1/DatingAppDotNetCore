using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTO
    {
        [Required, MinLength(3,ErrorMessage ="atleast 3 characters are required"),MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
