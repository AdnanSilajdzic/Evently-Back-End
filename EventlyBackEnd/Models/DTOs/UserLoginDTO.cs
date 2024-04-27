using EventlyBackEnd.Models.Entities;

namespace EventlyBackEnd.Models.DTOs
{
    public class UserLoginDTO
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
