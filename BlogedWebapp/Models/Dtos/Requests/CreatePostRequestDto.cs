using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogedWebapp.Models.Dtos.Requests
{
    public class CreatePostRequestDto
    {
        [Required]
        public string AuthorId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string UrlName { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string CategoryId { get; set; }

        public List<string> Tags { get; set; }

    }
}
