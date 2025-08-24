using Microsoft.AspNetCore.Mvc;
using SecureNotesWebApi.Context;
using SecureNotesWebApi.Models;

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
    }
}
