using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        public string BlogId { get; set; }

    }
}
