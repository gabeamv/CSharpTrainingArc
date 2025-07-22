using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGames
{
    public class Deck
    {
        // This is a static field so that it can be reused across instances of Deck.
        // If you create multiple Random instances, they may produce less random results.
        private static Random random = new Random();
        private List<Card> deck;
        public Deck()
        {
            deck = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    deck.Add(new Card(suit, rank));
                }
            }
        }

        // Fisher-Yates shuffle algorithm
        // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        public void Shuffle()
        {
            for (int i = deck.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                Card temp_i = deck[i];
                deck[i] = deck[j];
                deck[j] = temp_i;
            }
        }

        
        public Card? DrawCard()
        {
            if (deck.Count == 0) return null;
            return deck[deck.Count - 1];
        }
        
        public override string ToString()
        {
            StringBuilder deckStringBuilder = new StringBuilder();
            foreach (Card card in deck) deckStringBuilder.Append(card);
            return deckStringBuilder.ToString();
        }
    }
}
