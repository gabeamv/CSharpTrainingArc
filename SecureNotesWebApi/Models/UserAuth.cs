namespace SecureNotesWebApi.Models
{
    public class UserAuth
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string HashedPassword { get; set; }
        
        //public string? PublicKey { get; set; }
    }
}
