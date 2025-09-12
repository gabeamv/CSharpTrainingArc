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
        public byte[] AesDecryptBytes(byte[] cipherText)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            string decryption = null;


            ICryptoTransform decryptor = AesAlg.CreateDecryptor(AesAlg.Key, AesAlg.IV);

            using (MemoryStream msDecrypt = new())
            {
                using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Write))
                {
                    csDecrypt.Write(cipherText, 0, cipherText.Length);
                    csDecrypt.FlushFinalBlock();
                    return msDecrypt.ToArray();
                }
            }

        }
        public byte[] RsaEncryptBytes(byte[] bytes, string publicKeyPem)
        {
            byte[] cipherText;
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(publicKeyPem);
                cipherText = rsa.Encrypt(bytes, RSAEncryptionPadding.OaepSHA256);
            }
            return cipherText;
        }

        public byte[] RsaDecryptBytes(byte[] bytes, string privateKeyPem)
        {
            byte[] plainTextBytes;
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(privateKeyPem);
                plainTextBytes = rsa.Decrypt(bytes, RSAEncryptionPadding.OaepSHA256);
            }
            return plainTextBytes;
        }

        public (byte[] ciphertext, byte[] key, byte[] iv, byte[] tag) AesGcmEncrypt(byte[] plaintext)
        {
            byte[] ciphertext = new byte[plaintext.Length];
            byte[] iv = RandomNumberGenerator.GetBytes(12);
            byte[] tag = new byte[16];
            byte[] key = RandomNumberGenerator.GetBytes(32);
            using (AesGcm AesGcmAlg = new AesGcm(key, 16))
            {
                AesGcmAlg.Encrypt(iv, plaintext, ciphertext, tag);
                return (ciphertext, key, iv, tag);
            }
        }

        public byte[] AesGcmDecrypt(byte[] ciphertext, byte[] key, byte[] iv, byte[] tag)
        {
            byte[] plaintext = new byte[ciphertext.Length];
            using (AesGcm AesGcmAlg = new AesGcm(key, 16))
            {
                AesGcmAlg.Decrypt(iv, ciphertext, tag, plaintext);
                return plaintext;
            }
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
