using CardGames.Games;

namespace CardGames
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BlackJackGame blackJackGame = new BlackJackGame();
            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("Welcome to Card Games!");
                int choice = GameChoice();
                switch (choice)
                {
                    case 1:
                        blackJackGame.Start();
                        break;
                    default:
                        Console.WriteLine("Thank you for playing!");
                        break;
                }
            }
        }

        private static int GameChoice()
        {
            Console.WriteLine("Select which game to play: ");
            DisplayChoices();
            string choice = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(choice) || !int.TryParse(choice, out int gameChoice) || gameChoice < 1 || gameChoice > 1)
            {
                Console.WriteLine("Invalid choice. Please enter a number corresponding to the game you want to play.");
                DisplayChoices();
                choice = Console.ReadLine();
            }
            return int.Parse(choice);
        }

        private static void DisplayChoices()
        {
            Console.WriteLine("1. Blackjack");
        }
    }

}
