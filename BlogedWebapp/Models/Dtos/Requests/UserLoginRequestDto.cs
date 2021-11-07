using System.ComponentModel.DataAnnotations;

namespace BlogedWebapp.Models.Dtos.Requests
{
    public class UserLoginRequestDto
    {

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
