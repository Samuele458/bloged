using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogedWebapp.Models
{
    public class RegisterRequest
    {
        [Required]
        public String Username { get; set; }

        [Required]
        public String Password { get; set; }

        [Required]
        public String FirstName { get; set; }

        [Required]
        public String LastName { get; set; }
    }
}
