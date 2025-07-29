using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureNotes.IServices
{
    public interface IEncryptDecryptService
    {
        public byte[] AesEncryptBytes(byte[] bytes);
        public string AesDecryptBytes(byte[] cipherText, byte[] Key, byte[] IV);
    }
}
