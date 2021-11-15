using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogedWebapp.Models.Dtos.Responses
{
    /// <summary>
    ///  Response Data Transfer Object for generic uses.
    /// </summary>
    public class GenericResponseDto
    {
        public bool Success { get; set; }

        public List<string> Errors { get; set; }
    }
}
