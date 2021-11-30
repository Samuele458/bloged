namespace BlogedWebapp.Entities
{
    public class Post : OwnableEntity<Blog>
    {
        public string Title { get; set; }

        public string UrlName { get; set; }

        public string Content { get; set; }


    }
}
