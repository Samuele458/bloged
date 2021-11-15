using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogedWebapp.Models.Dtos.Generic
{
    public class RoleDto
    {

        [Required]
        public string RoleName { get; set; }
    }
}
