namespace BlogedWebapp.Models.Dtos.Requests
{
    public class UpdatePostRequestDto
    {
        public string Title { get; set; }

        public string UrlName { get; set; }

        public string Content { get; set; }

        public string CategoryId { get; set; }

    }
}
