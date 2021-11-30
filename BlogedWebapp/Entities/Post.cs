using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogedWebapp.Entities
{
    public class Post : OwnableEntity<Blog>
    {
        public string Title { get; set; }

        public string UrlName { get; set; }

        public string Content { get; set; }

        public string AuthorId { get; set; }
        
        [ForeignKey(nameof(AuthorId))]
        [JsonIgnore]
        public AppUser Author { get; set; }
    }
}
