using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace EncryptOrDie
{
    public class Program
    {
        static void Main(string[] args)
        {
     
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
            Console.WriteLine($"{DateTime.Now} - Rescue: Don't let him get access to your message! " +
                $"Change frequencies fast while the \nmessage is encrypting! If he guesses the " +
                $"same frequency, he will intercept it! (1-5)");

            FrequencyFight freqFight = new FrequencyFight(userName);
            freqFight.StartFight();
            
            if (!freqFight.userAlive)
            {
                Console.WriteLine($"{DateTime.Now} - Killer: Your coordinates are:\n{userMessage.ToString()}");
                Console.WriteLine($"{DateTime.Now} - Killer: I've found you! Your location is:\n{userMessage}");
                Console.WriteLine("The killer has found you. You are dead...");
            }
            else
            {
                // Create AES instance and encrypt/decrypt message.
                Aes aes = Aes.Create();

                byte[] encryptedMessage = EncryptorDecryptor.AesEncryptStringToBytes(userMessage.ToString(), aes.Key, aes.IV);
                Console.WriteLine($"Your encrypted message:");
                foreach (byte b in encryptedMessage) Console.Write(b);
                Console.WriteLine();

                string decryptedMessage = EncryptorDecryptor.AesDecryptStringFromBytes(encryptedMessage, aes.Key, aes.IV);
                Console.WriteLine($"Your decrypted message:\n{decryptedMessage}");

                Console.WriteLine($"{DateTime.Now} - Rescue: We're on the way, hang in there!");
                Console.WriteLine("You were eventually rescued.");
            }
            Console.WriteLine("Press Enter To Terminate.");
            Console.ReadLine();

        }
    
    }
    
}

