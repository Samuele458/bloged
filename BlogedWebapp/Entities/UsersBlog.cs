using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogedWebapp.Entities
{
    public enum BlogRoles
    {
        Owner,
        Admin,
        Moderator,
        Writer,
        Subscriber
    }

    public class UsersBlog : UserOwnableEntity
    {
        /*public Guid Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public AppUser User { get; set; }
        */

        public string BlogId { get; set; }

        [ForeignKey(nameof(BlogId))]
        [JsonIgnore]
        public Blog Blog { get; set; }

        public BlogRoles Role { get; set; }
    }
}
