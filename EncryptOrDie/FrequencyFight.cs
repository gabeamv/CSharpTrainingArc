using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptOrDie
{
    internal class FrequencyFight
    {
        public bool userAlive { get; set; }
        public string userName { get; set; }

        private Random random = new Random();
        public FrequencyFight(string name)
        {
            userName = name;
        }
        public void StartFight()
        {
            userAlive = true;

            for (int i = 0; i < 5; i++)
            {
                int killerNum = random.Next(1,6);

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
                    userAlive = false;
                    break;
                }
            }
        }
    }
}
