using EventlyBackEnd.Models.Entities;

namespace EventlyBackEnd.Models.DTOs
{
    public class PostDTO
    {
        public long PostId { get; set; }
        public long? PosterId { get; set; }
        public string? Description { get; set; }
        public string ImageURL { get; set; }
        public long EventId { get; set; }
    }
}
