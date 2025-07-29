using SecureNotes.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Security.Cryptography.Xml;

namespace SecureNotes.Services
{
    public class EncryptDecryptService : IEncryptDecryptService
    {
        private Aes AesAlg { get; } = Aes.Create();
        public EncryptDecryptService() 
        { 

        }

        public byte[] AesEncryptBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0) 
                throw new ArgumentNullException(nameof(bytes));
            byte[] encryption;

            using (Aes aesAlg = Aes.Create())
            {
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new())
                {
                    using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(bytes, 0, bytes.Length);
                        csEncrypt.FlushFinalBlock();
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        public string AesDecryptBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0) throw new ArgumentNullException(nameof(IV));
            string decryption = null; 

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = AesAlg.Key;
                aesAlg.IV = AesAlg.IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new(cipherText))
                {
                    using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader swDecrypt = new(csDecrypt))
                        {
                            decryption = swDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return decryption;
        }

        
    }
}
