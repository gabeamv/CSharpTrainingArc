using SurvivalRpg.Entities;
using SurvivalRpg.PlayableClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalRpg.Game
{
    public class Encounter
    {
        private static Random random = new Random();
        private Player _user;
        private Enemy _enemy;
        private bool inEncounter = true;
        public Encounter(Player user, Enemy enemy)
        {
            _user = user;
            _enemy = enemy;
        }

        public void Start()
        {
            while (_user.IsAlive() && _enemy.IsAlive())
            {
                int move = PromptMove();
                switch (move)
                {
                    case 1:
                        _user.Attack1(_enemy);
                        break;
                    case 2:
                        _user.Attack2(_enemy);
                        break;
                    case 3:
                        _user.Special1(_enemy);
                        break;
                    case 4:
                        _user.Special2(_enemy);
                        break;
                    default: break;
                }
                if (_enemy.IsAlive())
                {
                    int enemyChoice = random.Next(1, 3);
                    switch (enemyChoice) 
                    {
                        case 1:
                            _enemy.Special1(_user);
                            break;
                        case 2:
                            _enemy.Special2(_user);
                            break;
                        default: break;
                    }
                }
            }
        }

        public int PromptMove()
        {
            Console.WriteLine("Choose a move:\n1. Punch\n2. Kick\n3. Special One\n4. Special 2");
            string userInput = Console.ReadLine();
            int moveNum;
            while(!int.TryParse(userInput, out moveNum) || moveNum < 1 || moveNum > 4)
            {
                Console.WriteLine("Please choose a valid move:\n1. Punch\n2. Kick\n3. Special One\n4. Special 2");
                userInput = Console.ReadLine();
            }
            return moveNum;
        }
    }
}
