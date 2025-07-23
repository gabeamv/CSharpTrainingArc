using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGames
{
    public class Deck : IEnumerable<Card>
    {
        // This is a static field so that it can be reused across instances of Deck.
        // If you create multiple Random instances, they may produce less random results.
        private static Random random = new Random();
        public List<Card> Cards;
        public Deck()
        {
            Cards = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    Cards.Add(new Card(suit, rank));
                }
            }
            
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) GetEnumerator();
        }
        public IEnumerator<Card> GetEnumerator()
        {
            return Cards.GetEnumerator();
        }

        // Fisher-Yates shuffle algorithm
        // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        public void Shuffle()
        {
            for (int i = Cards.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                Card temp_i = Cards[i];
                Cards[i] = Cards[j];
                Cards[j] = temp_i;
            }
        }

        public void AddCard(Card card)
        {
            Cards.Add(card);
        }
        
        public Card? DrawCard()
        {
            if (Cards.Count == 0) return null;
            Card cardDraw = Cards[Cards.Count - 1];
            Cards.RemoveAt(Cards.Count - 1);
            return cardDraw;
        }
        
        public override string ToString()
        {
            StringBuilder deckStringBuilder = new StringBuilder();
            foreach (Card card in Cards) deckStringBuilder.Append(card);
            return deckStringBuilder.ToString();
        }
    }
}
