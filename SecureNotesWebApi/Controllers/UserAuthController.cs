using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureNotesWebApi.Context;
using SecureNotesWebApi.Models;
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

        // Method to register a new user.
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserAuth userAuth)
        {
            var duplicateUser = await _context.UserAuths.FirstOrDefaultAsync(u => u.Username == userAuth.Username);
            if (duplicateUser != null)
            {
                return Conflict($"There is already a user:\n{userAuth}");
            }
            await _context.UserAuths.AddAsync(userAuth);
            await _context.SaveChangesAsync();
            return Ok($"Successfully registered user:\n{userAuth}");
        }

        // Method to login a user.
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserAuth userAuth)
        {
            var user = await _context.UserAuths.FirstOrDefaultAsync(u => u.Username == userAuth.Username);
            // TODO: Implement JWT
            if (user != null && user.HashedPassword == userAuth.HashedPassword) return Ok($"Successfully logged in user:\n{userAuth}");
            return NotFound($"There is no such user:\n{userAuth}.");
        }

        // Method to delete an account.

    }
}
