﻿using Microsoft.AspNetCore.Identity;
using System;

namespace BlogedWebapp.Entities
{
    /// <summary>
    ///  User entity
    /// </summary>
    public class User : BaseEntity
    {

        public IdentityUser Identity { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

    }
}
