namespace SecureNotes.Models
{

    public class UserAuth
    {
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public string? PublicKey { get; set; }

        public UserAuth(string username, string hashedPassword, string? publicKey = null)
        {
            Username = username;
            HashedPassword = hashedPassword;
            PublicKey = publicKey;
        }

        public override string ToString()
        {
            return Username;
        }
    }
}
