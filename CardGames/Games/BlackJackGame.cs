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
        private const string HIT = "h";
        private const string STAND = "s";
        private const int BLACKJACK = 21;

        private List<Card> UserHand;
        private List<Card> DealerHand;
        private Deck Deck;
        public bool IsRunning { get; private set; } = false;

        public BlackJackGame()
        {
            Deck = new Deck();
            UserHand = new List<Card>();
            DealerHand = new List<Card>();
        }

        public override void Start()
        {
            IsRunning = true;
            while (IsRunning)
            {
                Console.WriteLine("Welcome to Blackjack!");
                Console.WriteLine("Shuffling the deck...");
                Deck.Shuffle();
                Console.WriteLine("Dealing cards...");
                Console.WriteLine();
                PlayTurn();
                ReturnCards();
                IsRunning = PlayAgain();
            }

        }

        protected override void Deal(List<Card> hand, Deck deck)
        {
            hand.Add(deck.DrawCard() ?? throw new InvalidOperationException("No more cards in the deck."));
        }

        protected override void PlayTurn()
        {
            for (int i = 0; i < 2; i++)
            {
                Deal(UserHand, Deck);
                Deal(DealerHand, Deck);
            }
            Console.WriteLine("Your hand:");
            DisplayHand(UserHand);
            Console.WriteLine("Dealer's hand:");
            DisplayHand(DealerHand.Take(1).ToList()); // Show only one dealer card
            string userChoice = PromptHit();

            if (userChoice == HIT && !IsBlackJack(UserHand))
            {
                do
                {
                    Deal(UserHand, Deck);
                    if (IsBust(UserHand) || IsBlackJack(UserHand)) break;
                    Console.WriteLine("Your hand:");
                    DisplayHand(UserHand);
                    Console.WriteLine("Dealer's hand:");
                    DisplayHand(DealerHand.Take(1).ToList()); // Show only one dealer card
                    userChoice = PromptHit();

                } while (userChoice == HIT);
            }

            if (IsBust(UserHand))
            {
                Console.WriteLine("You busted! Your hand value is over 21. Dealer wins!");
                Console.WriteLine("Your final hand:");
                DisplayHand(UserHand);
                Console.WriteLine();
                Console.WriteLine("Dealer's final hand:");
                DisplayHand(DealerHand);
            }

            else if (IsBlackJack(UserHand))
            {
                Console.WriteLine("Blackjack! You win!");
                Console.WriteLine("Your final hand:");
                DisplayHand(UserHand);
                Console.WriteLine();
                Console.WriteLine("Dealer's final hand:");
                DisplayHand(DealerHand);
            }

            else
            {
                while (!CompareHand(DealerHand, UserHand) && !IsBust(DealerHand) && !IsBlackJack(DealerHand))
                {
                    Deal(DealerHand, Deck);
                }

                if (IsBust(DealerHand)) { Console.WriteLine("Dealer busted! You win!"); }
                else if (IsBlackJack(DealerHand) && !IsBlackJack(UserHand)) { Console.WriteLine("Dealer has Blackjack! Dealer wins!"); }
                else Console.WriteLine("Dealer has a larger legal hand than yours! Dealer wins!");
                Console.WriteLine("Your final hand:");
                DisplayHand(UserHand);
                Console.WriteLine("Dealer's final hand:");
                DisplayHand(DealerHand);
            }
        }

        protected override bool IsGameOver()
        {
            return true;
        }
        public override void End()
        {

        }

        private string PromptHit()
        {
            Console.WriteLine("Hit or Stand? (h/s)");
            string userChoice = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(userChoice) ||
                (userChoice.ToLower() != HIT && userChoice.ToLower() != STAND))
            {
                Console.WriteLine("Invalid choice. Please enter 'h' to hit or 's' to stand.");
                userChoice = Console.ReadLine();
            }
            Console.WriteLine();
            return userChoice.ToLower();
        }

        private (int value, int aceElevenValue) HandValue(List<Card> hand)
        {
            int value = 0;
            int aceElevenValue = 0; // Used to calculate the value of aces when they are 11.
            foreach (Card card in hand)
            {
                if (card.Rank == Rank.Ace)
                {
                    value++;
                    aceElevenValue += 11; // Aces can be 1 or 11.
                }
                else if (card.Rank == Rank.Jack || card.Rank == Rank.Queen || card.Rank == Rank.King)
                {
                    value += 10;
                    aceElevenValue += 10;
                }
                else
                {
                    value += (int)card.Rank;
                    aceElevenValue += (int)card.Rank;
                }
            }
            return (value, aceElevenValue);
        }

        private bool IsBlackJack(List<Card> hand)
        {
            var (value, aceValue) = HandValue(hand);
            return (value == BLACKJACK || aceValue == BLACKJACK);
        }

        private bool IsBust(List<Card> hand)
        {
            var (value, aceElevenValue) = HandValue(hand);
            return value > BLACKJACK;
        }

        private void DisplayHand(List<Card> hand)
        {
            StringBuilder sbHand = new StringBuilder();
            (int value, int aceElevenValue) = HandValue(hand);
            if (aceElevenValue <= BLACKJACK) sbHand.Append($"Value: {aceElevenValue}\n");
            else sbHand.Append($"Value: {value}\n");
            foreach (Card card in hand) sbHand.Append(card.ToString() + "\n");
            Console.WriteLine(sbHand.ToString());
            // If there is only one card in the hand, it is dealer's hand upon initial deal without the second card.
            if (hand.Count == 1) Console.WriteLine("Another unknown card in dealer's hand.\n");
        }

        private void ReturnCards()
        {
            foreach (Card card in UserHand) Deck.AddCard(card);
            foreach (Card card in DealerHand) Deck.AddCard(card);
            UserHand.Clear();
            DealerHand.Clear();
        }

        public bool CompareHand(List<Card> handOne, List<Card> handTwo)
        {
            (int valueOne, int aceElevenValueOne) = HandValue(handOne);
            (int valueTwo, int aceElevenValueTwo) = HandValue(handTwo);

            int bestValueOne = aceElevenValueOne <= BLACKJACK ? aceElevenValueOne : valueOne;
            int bestValueTwo = aceElevenValueTwo <= BLACKJACK ? aceElevenValueTwo : valueTwo;

            return bestValueOne > bestValueTwo;
        }

        private bool PlayAgain()
        {
            Console.WriteLine("Would you like to play again? (y/n)");
            string userChoice = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(userChoice) && userChoice != "y" && userChoice != "n")
            {
                Console.WriteLine("Invalid choice. Please enter 'y' to play again or 'n' to exit.");
                userChoice = Console.ReadLine();
            }
            Console.WriteLine();
            return userChoice == "y";
        }
    }
}
