using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlogedWebapp.Entities
{
    public class UsersBlog : IIdentificableEntity
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public AppUser User { get; set; }


        public Guid BlogId { get; set; }

        [ForeignKey(nameof(BlogId))]
        [JsonIgnore]
        public Blog Blog { get; set; }


        public string Role { get; set; }
    }
}
