using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace EncryptOrDie
{
    public class Program
    {
        private const int SIZE = 2048;
        static void Main(string[] args)
        {
            /*
            RSA userRsa = RSA.Create(SIZE);
            string userPrivateKey = userRsa.ExportPkcs8PrivateKeyPem();
            string userPublicKey = userRsa.ExportSubjectPublicKeyInfoPem();
            RSA rescueRSA = RSA.Create(SIZE);
            string rescuePrivateKey = rescueRSA.ExportPkcs8PrivateKeyPem();
            string rescuePublicKey = rescueRSA.ExportSubjectPublicKeyInfoPem();
            */

            /*
            string test1 = "This is a test message.";
            try
            {
                Console.WriteLine("Before encryption: {0}", test1);
                using (Aes aes = Aes.Create())
                {
                    byte[] encrypted = EncryptStringToBytes(test1, aes.Key, aes.IV);
                    Console.WriteLine("Encryption:");
                    foreach (byte b in encrypted) Console.Write(b);
                    Console.WriteLine();
                    string roundTrip = DecryptStringFromBytes(encrypted, aes.Key, aes.IV);
                    Console.WriteLine("Decrytion Round Trip: ");
                    Console.WriteLine(roundTrip.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            */

            Console.WriteLine($"{DateTime.Now} - Rescue: Hello anybody there? What is your name?");
            Console.Write("Name: ");
            string userName = Console.ReadLine();
            while (userName == null)
            {
                Console.WriteLine($"{DateTime.Now} - Rescue: Sorry, I didn't get that. Can you repeat?");
                Console.Write("Name: ");
                userName = Console.ReadLine();
            }
            Console.WriteLine($"{userName}: My name is {userName}. I need you to pick me up.");
            bool isRunning = true;

            Console.WriteLine($"{DateTime.Now} - Rescue: Where are you at right now? Input coordinates (1-99).");

            Console.Write("X: ");
            string xCoord = Console.ReadLine();
            int xCoordInt;
            while (!int.TryParse(xCoord, out xCoordInt) || xCoordInt < 1 || xCoordInt > 99)
            {
                Console.WriteLine($"{DateTime.Now} - Rescue: Input a valid X coordinate (1-99)");
                Console.Write("X: ");
                xCoord = Console.ReadLine();
            }
            Console.Write("Y: ");
            string yCoord = Console.ReadLine();
            int yCoordInt;
            while (!int.TryParse(yCoord, out yCoordInt) || yCoordInt < 1 || yCoordInt > 99)
            {
                Console.WriteLine($"{DateTime.Now} - Rescue: Input a valid Y coordinate (1-99)");
                Console.Write("Y: ");
                yCoord = Console.ReadLine();
            }

            StringBuilder userMessage = new StringBuilder();
            userMessage.Append($"X: {xCoordInt}\nY: {yCoordInt}");

            Console.WriteLine($"{DateTime.Now} - Killer: I am going to find you and kill you...");
            Console.WriteLine($"{DateTime.Now} - Rescue: Don't let him get access to you message! " +
                $"Change frequencies fast while the \nmessage is encrypting! If he guesses the " +
                $"same frequency, he will intercept it! (1-5)");

            bool isAlive = true;

            for (int i = 0; i < 5; i++)
            {
                int killerNum = new Random().Next(1, 6);

                Console.Write("Frequency: ");
                string freq = Console.ReadLine();
                int freqNum;
                while (!int.TryParse(freq, out freqNum) || (freqNum < 1) || (freqNum > 5))
                {
                    Console.WriteLine("Invalid frequency. Input frequency again (1-5).");
                    Console.Write("Frequency: ");
                    freq = Console.ReadLine();
                }

                Console.WriteLine($"{userName}'s frequency: {freqNum}\nKiller's frequency: " +
                    $"{killerNum}");

                if (freqNum == killerNum)
                {
                    Console.WriteLine($"Your coordinates are:\n{userMessage.ToString()}");
                    Console.WriteLine($"{DateTime.Now} - Killer: I've found you! Your location is:\n{userMessage}");
                    isAlive = false;
                    break;
                }   
            }
            if (!isAlive)
            {
                Console.WriteLine("The killer has found you. You are dead...");
            }
            else
            {
                // Create AES instance and encrypt/decrypt message.
                Aes aes = Aes.Create();

                byte[] encryptedMessage = EncryptStringToBytes(userMessage.ToString(), aes.Key, aes.IV);
                Console.WriteLine($"Your encrypted message:");
                foreach (byte b in encryptedMessage) Console.Write(b);
                Console.WriteLine();

                string decryptedMessage = DecryptStringFromBytes(encryptedMessage, aes.Key, aes.IV);
                Console.WriteLine($"Your decrypted message:\n{decryptedMessage}");

                Console.WriteLine($"{DateTime.Now} - Rescue: We're on the way, hang in there!");
                Console.WriteLine("You were eventually rescued.");
            }
            Console.WriteLine("Press Enter To Terminate.");
            Console.ReadLine();

        }
        // Learn.Microsoft.com guide on how to encrypt a string.
        // https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.cryptostream?view=net-9.0
        public static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
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
        public static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
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

