namespace SecureNotes.Models
{

    public class UserAuth
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string? PublicKey { get; set; }
        public string? Salt { get; set; }

        public UserAuth(string username, string password, string? publicKey = null, string? salt = null)
        {
            Username = username;
            Password = password;
            PublicKey = publicKey;
            Salt = salt;
        }

        public override string ToString()
        {
            return Username;
        }
    }
}
