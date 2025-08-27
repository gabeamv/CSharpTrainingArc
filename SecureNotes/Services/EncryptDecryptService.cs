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
        // Static aes algorithm that persists throughout the lifetime of the application.
        public static Aes AesAlg { get; private set; } = Aes.Create();
        // TODO: Implement AesGcm
        public EncryptDecryptService() {
            // AesAlg.Padding = PaddingMode.None;
            
        }

        public byte[] AesEncryptBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0) 
                throw new ArgumentNullException(nameof(bytes));
            byte[] encryption;

            ICryptoTransform encryptor = AesAlg.CreateEncryptor(AesAlg.Key, AesAlg.IV);

            using (MemoryStream msEncrypt = new())
            {
                using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(bytes, 0, bytes.Length); // encrypt data
                    csEncrypt.FlushFinalBlock(); // finalize encryption process: last block, padding
                    return msEncrypt.ToArray(); // store in an array of bytes
                }
            }
            
        }

        public byte[] RsaEncryptBytes(byte[] bytes, string publicKeyPem)
        {
            byte[] cipherText;
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportFromPem(publicKeyPem);
                cipherText = RSA.Encrypt(bytes, true);
            }
            return cipherText;
        }

        public byte[] RsaDecryptBytes(byte[] bytes, string privateKeyPem)
        {
            byte[] plainTextBytes;
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportFromPem(privateKeyPem);
                plainTextBytes = RSA.Decrypt(bytes, true);
            }
            return plainTextBytes;
        }

        public string AesDecryptBytes(byte[] cipherText)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            string decryption = null; 

            
            ICryptoTransform decryptor = AesAlg.CreateDecryptor(AesAlg.Key, AesAlg.IV);

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
 
            return decryption;
        }


        public static void ChangeAesKey(byte[] key = null, byte[] iv = null, PaddingMode mode = PaddingMode.None)
        {

            if (key != null) AesAlg.Key = key;
            if (iv != null) AesAlg.IV = iv;
            if (mode != PaddingMode.None) AesAlg.Padding = mode;
        }

        public static void GenerateAes()
        {
            AesAlg = Aes.Create();
        }

        public static string GetKeyIV()
        {
            return $"{Convert.ToBase64String(AesAlg.Key)},{Convert.ToBase64String(AesAlg.IV)}";
        }
    }
}
