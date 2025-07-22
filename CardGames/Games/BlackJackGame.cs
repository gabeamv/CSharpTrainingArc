using CardGames.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGames.Games
{
    public class BlackJackGame : BaseCardGame, ICompare
    {
        List<Card> UserHand;
        List<Card> DealerHand;
        Deck Deck;
        public BlackJackGame() 
        { 
            Deck = new Deck();
            UserHand = new List<Card>();
            DealerHand = new List<Card>();
        }


        public override void Start()
        {
            Console.WriteLine("Welcome to Blackjack!");
            Console.WriteLine("Shuffling the deck...");
            Deck.Shuffle();
            
        }

        protected override void Deal()
        {
            
        }
        
        protected override void PlayTurn()
        {

        }

        protected override bool IsGameOver()
        {
            return true;
        }
        public override void End()
        {

        }

        bool ICompare.CompareHand( List<Card> userHand, List<Card> dealerHand)
        {
            return true;
        }
    }
}
