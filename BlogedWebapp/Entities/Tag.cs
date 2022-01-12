using BlogedWebapp.Helpers;

namespace BlogedWebapp.Entities
{
    public class Tag : OwnableEntity<Blog>
    {
        [Projection(ProjectionBehaviour.Preview)]
        public string Title { get; set; }

        [Projection(ProjectionBehaviour.Normal)]
        public string Description { get; set; }

        [Projection(ProjectionBehaviour.Preview)]
        public string UrlName { get; set; }
    }
}
