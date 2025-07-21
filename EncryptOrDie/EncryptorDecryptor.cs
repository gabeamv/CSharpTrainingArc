using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptOrDie
{
    public class EncryptorDecryptor
    {
        public EncryptorDecryptor()
        {

        }

        // Learn.Microsoft.com guide on how to encrypt a string.
        // https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.cryptostream?view=net-9.0
        public static byte[] AesEncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {

            // Argument validation.
            if (plainText == null || plainText.Length == 0) throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0) throw new ArgumentNullException(nameof(key));
            if (iv == null) throw new ArgumentNullException(nameof(iv));
            byte[] encrypted;

            // Create an AES object with the specified key and initialization vector.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create an encryptor to perform the stream transformation.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                // MemoryStream holds the encrypted data in memory.
                using (MemoryStream msEncrypt = new())
                {
                    // CryptoStream takes the MemoryStream and applies the encryption transformation to any
                    // data written into it.
                    using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // Writes the original plain text into the CryptoStream which encrypts the data
                        // and stores the encrypted data in the MemoryStream
                        using (StreamWriter swEncrypt = new(csEncrypt))
                        {
                            // Write all the data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the stream of memory.
            return encrypted;
        }
        // Learn.Microsoft.com guide on how to decrypt cipher text.
        // https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.cryptostream?view=net-9.0
        public static string AesDecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Argument validation.
            if (cipherText == null || cipherText.Length == 0) throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length <= 0) throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0) throw new ArgumentNullException(nameof(iv));

            // Declare the string used to hold the decrypted ciphertext
            string plaintext = null;

            // Create an AES object with  the specified key and initialization vector.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decryptor to perform the stream transformation algorithm.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                // MemoryStream to hold the decrypted data in memory.
                using (MemoryStream msDecrypt = new(cipherText))
                {
                    // CryptoStream to take the MemoryStream and apply the decryption transformation when
                    // data is read.
                    using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        // StreamReader to read the ciphertext into the cryptoStream to decrypt the data and 
                        // store it in memory stream
                        using (StreamReader swDecrypt = new(csDecrypt))
                        {
                            // Read the data and store in the stream.
                            plaintext = swDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
