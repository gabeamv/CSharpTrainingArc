using CardGames.Games;

namespace CardGames
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BlackJackGame blackJackGame = new BlackJackGame();
            blackJackGame.Start();
        }
    }
}
