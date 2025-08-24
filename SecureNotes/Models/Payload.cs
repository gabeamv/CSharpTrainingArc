using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureNotes.Models
{
    public class Payload
    {
        public string UUID { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Ciphertext { get; set; }
        public string Key { get; set; }
        public string IV { get; set; }
        public string Format { get; set; }
        public DateTime Timestamp { get; set; }

        public Payload()
        {

        }
    }
}
