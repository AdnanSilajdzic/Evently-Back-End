using Microsoft.Extensions.Hosting;

namespace EventlyBackEnd.Models.Entities
{
    public class Event
    {
        public long EventId { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public string? Name { get; set; }
        public DateTime DateTime { get; set; }
        public string? Location { get; set; }
        public string? Type { get; set; }
        public bool Featured { get; set; }

    }
}
