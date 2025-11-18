using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureNotes.Models
{
    public class Secret
    {
        public string _CiphertextKey { get; }
        public string _IV { get; }
        public string _Tag { get; }
        public string _CiphertextMessage { get; }
        public Secret(string ciphertextKey, string iv, string tag, string ciphertextMessage)
        {
            _CiphertextKey = ciphertextKey;
            _IV = iv;
            _Tag = tag;
            _CiphertextMessage = ciphertextMessage;
        }
    }
}
