using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventlyBackEnd.Functions;
using EventlyBackEnd.Models.Entities;
using EventlyBackEnd.Models.DTOs;
using AutoMapper;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace EventlyBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly EventlyDbContext _context;
        private Authenticate _Authenticate;

        public UsersController(EventlyDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _Authenticate = new Authenticate();
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<ActionResult<UserWithToken>> PostUser(UserDTO userDto)
        {
            // Map UserDTO to User entity
            var user = _mapper.Map<User>(userDto);
            // Hash the password
            string hashedPassword = _Authenticate.HashPassword(user.Password);

            // Replace the user's plain text password with the hashed one
            user.Password = hashedPassword;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            string jwtToken = _Authenticate.GenerateJwtToken(user.Id, user.Email, Environment.GetEnvironmentVariable("SecretKey"));
            return new UserWithToken { User = user, Token = jwtToken };
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDTO loginModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginModel.Email);

            if (user == null || !_Authenticate.VerifyPassword(loginModel.Password, user.Password))
            {
                // Invalid username or password
                return Unauthorized();
            }

            string jwtToken = _Authenticate.GenerateJwtToken(user.Id, user.Email, Environment.GetEnvironmentVariable("SecretKey"));
            return Ok(new { user, Token = jwtToken });
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            // Extract and verify JWT token
            string token = authorizationHeader?.Split(' ').Last();
            if (string.IsNullOrEmpty(token) || !_Authenticate.VerifyJwtToken(token, Environment.GetEnvironmentVariable("SecretKey")))
            {
                // Invalid or missing token
                return Unauthorized();
            }

            // Token is valid, continue with delete operation
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Users/{userId}/Events
        [HttpGet("~/api/Users/{userId}/Events")]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetEventsByUser(long userId)
        {
            // Find the user by userId
            var user = await _context.Users.Include(u => u.CreatedEvents).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Retrieve the events associated with the user
            var events = user.CreatedEvents;

            // Map events to EventDTOs
            var eventDTOs = _mapper.Map<IEnumerable<EventDTO>>(events);

            return Ok(eventDTOs);
        }

        // POST: api/User/SaveEvent
        [HttpPost("SaveEvent/{eventId}")]
        public async Task<IActionResult> SaveEvent(long eventId, long userId)
        {
            try
            {
                // Find the user by user ID
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                // Find the event by event ID
                var @event = await _context.Events.FindAsync(eventId);
                if (@event == null)
                {
                    return NotFound("Event not found");
                }

                // Check if the user has already saved the event
                var existingSavedEvent = await _context.UserSavedEvents
                    .FirstOrDefaultAsync(use => use.UserId == userId && use.EventId == eventId);

                if (existingSavedEvent != null)
                {
                    // If the event is already saved, delete it from the user's saved events
                    _context.UserSavedEvents.Remove(existingSavedEvent);
                    await _context.SaveChangesAsync();

                    return Ok("Event removed from saved events");
                }

                // Save the event for the user
                var userSavedEvent = new UserSavedEvent
                {
                    UserId = userId,
                    EventId = eventId
                };

                _context.UserSavedEvents.Add(userSavedEvent);
                await _context.SaveChangesAsync();

                return Ok("Event saved successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // GET: api/User/SavedEvents/{userId}
        [HttpGet("SavedEvents/{userId}")]
        public async Task<IActionResult> GetSavedEvents(long userId)
        {
            try
            {
                // Retrieve the saved events for the user
                var savedEvents = await _context.UserSavedEvents
                    .Where(use => use.UserId == userId)
                    .Select(use => use.Event)
                    .ToListAsync();

                if (savedEvents == null || !savedEvents.Any())
                {
                    return NotFound("No saved events found for the user");
                }

                return Ok(savedEvents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Users/{userId}/Posts
        [HttpGet("~/api/Users/{userId}/Posts")]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetPostsByUser(long userId)
        {
            // Find the user by userId
            var user = await _context.Users.Include(u => u.Posts).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Retrieve the events associated with the user
            var posts = user.Posts;

            // Map events to EventDTOs
            var postDTOs = _mapper.Map<IEnumerable<PostDTO>>(posts);

            return Ok(postDTOs);
        }


        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
