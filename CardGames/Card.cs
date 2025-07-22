using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGames
{
    public enum Suit { 
        Hearts, Diamonds, Clubs, Spades 
    }
    public enum Rank
    {
        Ace = 1,
        Two, 
        Three, 
        Four, 
        Five, 
        Six, 
        Seven, 
        Eight, 
        Nine, 
        Ten, 
        Jack, 
        Queen, 
        King // 13
    }
    
    public class Card
    {
        public Suit Suit { get;}
        public Rank Rank { get;}

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}\n";
        }
    }
}
