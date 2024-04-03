﻿namespace EventlyBackEnd.Models
{
    public class User
    {
        public long Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Location { get; set; }
        public ICollection<Event> CreatedEvents { get; set; }

    }
}