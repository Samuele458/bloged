using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogedWebapp.Entities
{
    public class OwnableEntity : BaseEntity
    {

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public AppUser User { get; set; }

    }
}
