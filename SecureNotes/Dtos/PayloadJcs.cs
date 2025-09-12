using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureNotes.Dtos
{
    public class PayloadJcs
    {
        public string UUID { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Ciphertext { get; set; }
        public string Key { get; set; }
        public string IV { get; set; }
        public string Tag { get; set; }
        public string Format { get; set; }
        public string Timestamp { get; set; }

        public PayloadJcs()
        {

        }
    }
}
