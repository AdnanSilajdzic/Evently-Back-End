using AutoMapper;
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
    public class PostsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly EventlyDbContext _context;
        private Authenticate _Authenticate;

        public PostsController(EventlyDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _Authenticate = new Authenticate();
            _context = context;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await _context.Posts.ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(long id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<ActionResult<PostDTO>> CreatePost(PostDTO newPostDTO, [FromServices] IMapper mapper)
        {
            // Map the PostDTO to a Post entity
            var newPost = mapper.Map<Post>(newPostDTO);

            // Add the new Post entity to the context and save changes
            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            // Map the created Post entity back to a PostDTO
            var createdPostDTO = mapper.Map<PostDTO>(newPost);

            // Return the created PostDTO
            return Ok(createdPostDTO);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(long id, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            // Extract and verify JWT token
            string token = authorizationHeader?.Split(' ').Last();
            if (string.IsNullOrEmpty(token) || !_Authenticate.VerifyJwtToken(token, Environment.GetEnvironmentVariable("SecretKey")))
            {
                // Invalid or missing token
                return Unauthorized();
            }

            // Token is valid, continue with delete operation
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok("post deleted");
        }
    }
}
