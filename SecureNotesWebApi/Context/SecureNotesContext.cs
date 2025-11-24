using Microsoft.EntityFrameworkCore;
using SecureNotesWebApi.Models;
namespace SecureNotesWebApi.Context
{
    public class SecureNotesContext : DbContext
    {
        public SecureNotesContext(DbContextOptions<SecureNotesContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserAuth> UserAuths { get; set; }
        public DbSet<Payload> Messages { get; set; }

    }
}
