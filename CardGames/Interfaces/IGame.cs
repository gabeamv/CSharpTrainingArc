using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGames.Interfaces
{
    internal interface IGame
    {
        void Start();
        bool IsGameOver();
        void PlayTurn();
        void End();
    }
}
