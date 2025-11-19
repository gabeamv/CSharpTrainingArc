using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureNotes.Models
{
    public class Secret
    {
        public string CiphertextKey { get; }
        public string IV { get; }
        public string Tag { get; }
        public string CiphertextMessage { get; }
        public Secret(string ciphertextKey, string iv, string tag, string ciphertextMessage)
        {
            CiphertextKey = ciphertextKey;
            IV = iv;
            Tag = tag;
            CiphertextMessage = ciphertextMessage;
        }
    }
}
