using System.ComponentModel.DataAnnotations;

namespace SecureNotesWebApi.Models
{
    public class Payload
    {
        [Key]
        public required string ID { get; set; }
        public required string Sender { get; set; }
        public required string Recipient { get; set; }
        public required string Key { get; set; }
        public required string IV { get; set; }
        public required string Format { get; set; }
        public required DateTime Timestamp { get; set; }
    }
}
