using Microsoft.AspNetCore.Http;
using System;

namespace EventlyBackEnd.Models.DTOs
{
    public class CreateEventDTO
    {
        public long CreatorId { get; set; }
        public string? Name { get; set; }
        public DateTime DateTime { get; set; }
        public string? Location { get; set; }
        public string? Type { get; set; }
        public bool Featured { get; set; }
        public IFormFile Image { get; set; }
    }
}
