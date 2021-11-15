using System.Collections.Generic;

namespace BlogedWebapp.Models.Dtos.Responses
{
    public class AuthResponseDto : GenericResponseDto
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

    }
}
