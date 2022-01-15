using System.ComponentModel.DataAnnotations;

namespace BlogedWebapp.Models.Dtos.Requests
{
    public class AddTagToPostRequestDto
    {
        [Required]
        public string TagId { get; set; }
    }
}
