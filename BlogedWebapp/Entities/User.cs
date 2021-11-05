using System;

namespace BlogedWebapp.Entities
{
    public class User : BaseEntity
    {

        public String FirstName { get; set; }

        public String LastName { get; set; }

        public String Username { get; set; }

        public String Password { get; set; }

    }
}
