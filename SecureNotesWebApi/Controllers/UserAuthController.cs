using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureNotesWebApi.Context;
using SecureNotesWebApi.Models;
using System.Text.Json;
using Isopoh.Cryptography.Argon2;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using NuGet.Packaging.Signing;
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
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            Argon2Config config = new Argon2Config
            {
                Salt = salt,
                Password = Encoding.UTF8.GetBytes(userAuth.Password)
            };
            userAuth.Salt = Convert.ToBase64String(salt);
            userAuth.Password = Argon2.Hash(config);
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
            if (user != null)
            {
                Argon2Config config = new Argon2Config
                {
                    Salt = Convert.FromBase64String(user.Salt),
                    Password = Encoding.UTF8.GetBytes(userAuth.Password)
                };
                if (Argon2.Verify(user.Password, config)) return Ok($"Successfully logged in user.");
            }
            return Unauthorized($"There is no such user.");
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

        // Method to verify if a user is registered or not.
        [HttpGet("is_registered/{username}")]
        public async Task<IActionResult> CanRegister([FromRoute] string username)
        {
            var user = await _context.UserAuths
                .FirstOrDefaultAsync(_user => _user.Username == username);
            if (user == null) return Ok();
            return Conflict();
        }

    }
}
