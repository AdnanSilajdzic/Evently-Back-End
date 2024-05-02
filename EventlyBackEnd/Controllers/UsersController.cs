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

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
