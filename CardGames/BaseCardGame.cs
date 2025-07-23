using CardGames.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGames
{
    public abstract class BaseCardGame
    {
        public BaseCardGame()
        {

        }

        public abstract void Start();
        protected abstract bool IsGameOver();
        protected abstract void Deal(List<Card> hand, Deck deck);
        protected abstract void PlayTurn();
        public abstract void End();
        
    }
}
