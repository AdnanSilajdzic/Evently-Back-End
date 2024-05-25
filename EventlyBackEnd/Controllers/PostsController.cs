using Amazon.S3.Model;
using AutoMapper;
using EventlyBackEnd.Functions;
using EventlyBackEnd.Models.DTOs;
using EventlyBackEnd.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventlyBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly EventlyDbContext _context;
        private Authenticate _Authenticate;
        private readonly Backblaze image;

        public PostsController(EventlyDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _Authenticate = new Authenticate();
            _context = context;
            image = new Backblaze();
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

        // GET: api/Posts/Event/5
        [HttpGet("Event/{id}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostByEventId(long id)
        {
            var posts = await _context.Posts.Where(p => p.EventId == id).ToListAsync();
            return Ok(posts);
        }

        // PUT: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PostDTO>> CreatePost([FromForm] CreatePostDTO newPostDTO)
        {
            // Map the PostDTO to a Post entity
            var newPost = _mapper.Map<Post>(newPostDTO);
            // Upload image to Backblaze B2
            if (newPostDTO.Image != null)
            {
                try
                {
                    var imageUrl = await image.UploadImageToBackblaze(newPostDTO.Image, "posts");
                    newPost.ImageURL = imageUrl;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
            // Add the new Post entity to the context and save changes
            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            // Map the created Post entity back to a PostDTO
            var createdPostDTO = _mapper.Map<PostDTO>(newPost);

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
