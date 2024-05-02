namespace EventlyBackEnd.Models.Entities
{
    public class UserSavedEvent
    {
        public long UserId { get; set; }
        public User User { get; set; }

        public long EventId { get; set; }
        public Event Event { get; set; }
    }
}
