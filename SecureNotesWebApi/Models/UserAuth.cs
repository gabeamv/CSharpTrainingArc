using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SecureNotesWebApi.Models
{

    public class UserAuth
    {
        [Key]
        public required string Username { get; set; }

        public required string HashedPassword { get; set; }
        public string? PublicKey { get; set; }
    }
}
