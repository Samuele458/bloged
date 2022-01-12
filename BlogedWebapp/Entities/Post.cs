using BlogedWebapp.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogedWebapp.Entities
{
    public class Post : OwnableEntity<Blog>
    {
        [Projection(ProjectionBehaviour.Preview)]
        public string Title { get; set; }

        [Projection(ProjectionBehaviour.Preview)]
        public string UrlName { get; set; }

        [Projection(ProjectionBehaviour.Normal)]
        public string Content { get; set; }

        [Projection(ProjectionBehaviour.Preview)]
        public string AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        [JsonIgnore]
        [Projection(ProjectionBehaviour.Full)]
        public AppUser Author { get; set; }

        [Projection(ProjectionBehaviour.Preview)]
        public string CategoryId { get; set; }

        [Projection(ProjectionBehaviour.Normal)]
        [RelatedEntity]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        [Projection(ProjectionBehaviour.Normal)]
        [RelatedEntity]
        public virtual ICollection<PostsTag> Tags { get; set; }
    }
}
