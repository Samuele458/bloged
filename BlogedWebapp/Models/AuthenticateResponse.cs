using BlogedWebapp.Entities;
using System;

namespace BlogedWebapp.Models
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Username { get; set; }
        public String Token { get; set; }


        public AuthenticateResponse(User user, string token)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Token = token;
        }
    }
}
