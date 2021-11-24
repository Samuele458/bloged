using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;

namespace BlogedWebapp.Entities
{
    /// <summary>
    ///  Profile entity
    /// </summary>
    public class Profile : OwnableEntity
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}
