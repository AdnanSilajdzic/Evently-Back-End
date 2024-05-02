namespace EventlyBackEnd.Models.Entities
{
    public class Post
    {
        public long PostId { get; set; }
        public long? PosterId { get; set; }
        public User? Poster { get; set; }
        public string? Description { get; set; }
        public string ImageURL { get; set; }
        public long EventId { get; set; }
        public Event Event { get; set; }
    }
}
