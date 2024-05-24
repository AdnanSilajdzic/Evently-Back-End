using EventlyBackEnd.Models.Entities;

namespace EventlyBackEnd.Models.DTOs
{
    public class PostDTO
    {
        public long? PosterId { get; set; }
        public string? Description { get; set; }
        public IFormFile Image { get; set; }
        public long EventId { get; set; }
    }
}
