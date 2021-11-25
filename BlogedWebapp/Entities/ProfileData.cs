using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;

namespace BlogedWebapp.Entities
{
    /// <summary>
    ///  ProfileData entity
    /// </summary>
    public class ProfileData : OwnableEntity
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}
