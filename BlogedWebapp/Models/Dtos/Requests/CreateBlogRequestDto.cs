using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogedWebapp.Models.Dtos.Requests
{
    public class CreateBlogRequestDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string UrlName { get; set; }


    }
}
