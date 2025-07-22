using CardGames.Games;

namespace CardGames
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Deck deck = new Deck();
            Console.WriteLine(deck);
            deck.Shuffle();
            Console.WriteLine($"Shuffled Deck:\n{deck}");
            //BlackJackGame blackJack = new BlackJackGame();
        }
    }
}
