using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureNotesWebApi.Context;
using SecureNotesWebApi.Models;
using System.Text.Json;
namespace SecureNotesWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAuthController : ControllerBase
    {
        private readonly SecureNotesContext _context;
        public UserAuthController(SecureNotesContext context)
        {
            _context = context;
        }

        [HttpGet("get")]
        public IActionResult Get()
        {
            return Ok("API is working");
        }

        [HttpGet("get_users")]
        public async Task<ActionResult<List<UserAuth>>> GetUsers()
        {
            List<UserAuth> users = await _context.UserAuths.ToListAsync();
            return Ok(users);
        }

        // Method to register a new user.
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserAuth userAuth)
        {
            var user = await _context.UserAuths.FirstOrDefaultAsync(u => u.Username == userAuth.Username);
            if (user != null)
            {
                return Conflict($"There is already a user:\n{userAuth.PublicKey}");
            }
            await _context.UserAuths.AddAsync(userAuth);
            await _context.SaveChangesAsync();
            return Ok($"Successfully registered user:\n{userAuth.PublicKey}");
        }

        // Method to login a user.
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserAuth userAuth)
        {
            var user = await _context.UserAuths.FirstOrDefaultAsync(u => u.Username == userAuth.Username);
            // TODO: Implement JWT
            if (user != null && user.HashedPassword == userAuth.HashedPassword) return Ok($"Successfully logged in user:\n{JsonSerializer.Serialize(userAuth)}");
            return NotFound($"There is no such user:\n{JsonSerializer.Serialize(userAuth)}.");
        }

        // Method to get a user's public key.
        [HttpGet("get_public_key/{username}")]
        public async Task<ActionResult<string>> GetPublicKey([FromRoute] string username)
        {
            var user = await _context.UserAuths
                .FirstOrDefaultAsync(_user => _user.Username == username);
            if (user == null) return NotFound("No user found.");
            return Ok(user.PublicKey);
        }

    }
}
