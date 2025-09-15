using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureNotesWebApi.Context;
using SecureNotesWebApi.Models;
using System.Text.Json;
using Isopoh.Cryptography.Argon2;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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
                return Conflict($"There is already a user.");
            }
            userAuth.Salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            userAuth.Password = Argon2.Hash(userAuth.Password + userAuth.Salt);
            await _context.UserAuths.AddAsync(userAuth);
            await _context.SaveChangesAsync();
            return Ok($"Successfully registered user. Hashed password {userAuth.Password}");
        }

        // Method to login a user.
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserAuth userAuth)
        {
            var user = await _context.UserAuths.FirstOrDefaultAsync(u => u.Username == userAuth.Username);
            // TODO: Implement JWT
            if (user != null && user.Password == Argon2.Hash(userAuth.Password + user.Salt)) return Ok($"Successfully logged in user:\n{JsonSerializer.Serialize(userAuth)}");
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
