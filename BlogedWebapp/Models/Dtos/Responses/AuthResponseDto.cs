using System.Collections.Generic;

namespace BlogedWebapp.Models.Dtos.Responses
{
    public class AuthResponseDto
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public bool Success { get; set; }

        public List<string> Errors { get; set; }
    }
}
