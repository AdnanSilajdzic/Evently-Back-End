using EventlyBackEnd.Models.Entities;

namespace EventlyBackEnd.Models.DTOs
{
    public class EventDTO
    {
        public long EventId { get; set; }
        public long CreatorId { get; set; }
        public string? Name { get; set; }
        public DateTime DateTime { get; set; }
        public string? Location { get; set; }
        public string? Type { get; set; }
        public bool Featured { get; set; }
        public string ImageURL { get; set; }
    }
}
