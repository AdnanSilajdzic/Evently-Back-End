using Microsoft.Build.Evaluation;
using Microsoft.Extensions.Hosting;

namespace EventlyBackEnd.Models.Entities
{
    public class Event
    {
        public long EventId { get; set; }
        public long CreatorId { get; set; }
        public User Creator { get; set; }
        public string? Name { get; set; }
        public DateTime DateTime { get; set; }
        public string? Location { get; set; }
        public string? Type { get; set; }
        public bool Featured { get; set; }
        public string ImageURL { get; set; }
        public ICollection<UserSavedEvent> SavedByUsers { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
