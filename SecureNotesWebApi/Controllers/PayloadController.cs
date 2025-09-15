using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureNotesWebApi.Context;
using SecureNotesWebApi.Models;
using System.Security.Cryptography;

namespace SecureNotesWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayloadController : ControllerBase
    {
        private readonly SecureNotesContext _context;

        public PayloadController(SecureNotesContext context)
        {
            _context = context;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send(Payload payload)
        {
            await _context.Messages.AddAsync(payload);
            await _context.SaveChangesAsync();
            return Ok($"Payload '{payload.UUID}' has been sent.");
        }

        [HttpGet("received_messages/{username}")]
        public async Task<ActionResult<List<Payload>>> GetAllMessages([FromRoute] string username)
        {
            var userReceivedMessages = await _context.Messages
                .Where(message => message.Recipient == username)
                .ToListAsync();
            return Ok(userReceivedMessages);
        }
    }
}
