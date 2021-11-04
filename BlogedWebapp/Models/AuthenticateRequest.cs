using System;
using System.ComponentModel.DataAnnotations;

namespace BlogedWebapp.Models
{
    public class AuthenticateRequest
    {
        [Required]
        public String Username { get; set; }

        [Required]
        public String Password { get; set; }
    }
}
