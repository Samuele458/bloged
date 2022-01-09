using BlogedWebapp.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlogedWebapp.Entities
{
    public class Blog : BaseEntity
    {
        [Projection(ProjectionBehaviour.Preview)]
        public string Title { get; set; }

        [Projection(ProjectionBehaviour.Preview)]
        public string UrlName { get; set; }

        [Projection(ProjectionBehaviour.Normal)]
        public string Description { get; set; }

        [RelatedEntity]
        [Projection(ProjectionBehaviour.Normal)]
        [JsonIgnore]
        public virtual ICollection<UsersBlog> UsersBlog { get; set; }

        [RelatedEntity]
        [Projection(ProjectionBehaviour.Normal)]
        [JsonIgnore]
        public virtual ICollection<Post> Posts { get; set; }
    }
}
