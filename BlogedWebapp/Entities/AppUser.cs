using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BlogedWebapp.Entities
{
    public class AppUser : IdentityUser
    {
        //public string? ProfileDataId { get; set; }

        //[ForeignKey(nameof(ProfileDataId))]
        //[JsonIgnore]
        //public ProfileData ProfileData { get; set; }


        public virtual ICollection<UsersBlog> UsersBlog { get; set; }
    }
}
