using System.ComponentModel.DataAnnotations;

namespace BlogedWebapp.Models.Dtos.Requests
{
    public class CreateCategoryDto
    {

        [Required]
        public string Title { get; set; }

        [Required]
        public string UrlName { get; set; }

        public string Description { get; set; }
    }
}
