using BlogedWebapp.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogedWebapp.Entities
{
    public class PostsTag : BaseEntity
    {
        [Projection(ProjectionBehaviour.Preview)]
        public string TagId { get; set; }

        [ForeignKey(nameof(TagId))]
        [Projection(ProjectionBehaviour.Normal)]
        [RelatedEntity]
        public Tag Tag { get; set; }

        [Projection(ProjectionBehaviour.Preview)]
        public string PostId { get; set; }

        [ForeignKey(nameof(PostId))]
        [Projection(ProjectionBehaviour.Normal)]
        [RelatedEntity]
        public Post Post { get; set; }


    }
}
