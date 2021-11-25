using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogedWebapp.Entities
{
    public class OwnableEntity : BaseEntity
    {

        //ProfileData id when logged in
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public AppUser User { get; set; }

    }
}
