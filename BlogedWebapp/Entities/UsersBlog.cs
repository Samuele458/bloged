using BlogedWebapp.Helpers;
using Newtonsoft.Json;
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

        [Projection(ProjectionBehaviour.Preview)]
        public string BlogId { get; set; }

        [Projection(ProjectionBehaviour.Full)]
        [ForeignKey(nameof(BlogId))]
        [JsonIgnore]
        public Blog Blog { get; set; }

        [Projection(ProjectionBehaviour.Preview)]
        public BlogRoles Role { get; set; }
    }
}
