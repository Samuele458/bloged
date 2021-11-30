using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogedWebapp.Entities
{
    public class Blog : BaseEntity
    {
        public string Title { get; set; }

        public string UrlName { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<UsersBlog> UsersBlog { get; set; }

        [JsonIgnore]
        public virtual ICollection<Post> Posts { get; set; }
    }
}
