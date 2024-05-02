﻿using AutoMapper;
using EventlyBackEnd.Functions;
using EventlyBackEnd.Models.DTOs;
using EventlyBackEnd.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventlyBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly EventlyDbContext _context;
        private readonly IMapper _mapper;
        private Authenticate _Authenticate;

        public EventsController(EventlyDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _Authenticate = new Authenticate();
            _context = context;
        }

        // GET: api/Events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return await _context.Events.ToListAsync();
        }


        // GET: api/Event/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(long id)
        {
            var Event = await _context.Events.FindAsync(id);

            if (Event == null)
            {
                return NotFound();
            }

            return Event;
        }

        // POST: api/Events
        [HttpPost]
        public async Task<ActionResult<EventDTO>> PostEvent(EventDTO newEventDTO, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            // Extract and verify JWT token
            string token = authorizationHeader?.Split(' ').Last();
            if (string.IsNullOrEmpty(token) || !_Authenticate.VerifyJwtToken(token, Environment.GetEnvironmentVariable("SecretKey")))
            {
                // Invalid or missing token
                return Unauthorized();
            }

            var creator = await _context.Users.FindAsync(newEventDTO.CreatorId);
            if (creator == null)
            {
                return BadRequest("Invalid user ID");
            }

            var newEvent = _mapper.Map<Event>(newEventDTO);
            newEvent.Creator = creator;

            // Add the new event to the user's CreatedEvents collection
            creator.CreatedEvents ??= new List<Event>(); // Ensure the collection is initialized
            creator.CreatedEvents.Add(newEvent);

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            var createdEventDTO = _mapper.Map<EventDTO>(newEvent);
            return Ok(createdEventDTO);
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(long id, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            // Extract and verify JWT token
            string token = authorizationHeader?.Split(' ').Last();
            if (string.IsNullOrEmpty(token) || !_Authenticate.VerifyJwtToken(token, Environment.GetEnvironmentVariable("SecretKey")))
            {
                // Invalid or missing token
                return Unauthorized();
            }

            // Token is valid, continue with delete operation
            var Event = await _context.Events.FindAsync(id);
            if (Event == null)
            {
                return NotFound();
            }

            // Remove the event from the associated user's CreatedEvents collection
            var creator = await _context.Users.FindAsync(Event.CreatorId);
            if (creator != null)
            {
                creator.CreatedEvents.Remove(Event);
            }

            _context.Events.Remove(Event);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
