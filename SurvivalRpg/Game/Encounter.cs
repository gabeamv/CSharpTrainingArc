using SurvivalRpg.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalRpg.Game
{
    public class Encounter
    {
        private Player _user;
        private Enemy _enemy;
        private bool inEncounter = true;
        public Encounter(Player user, Enemy enemy)
        {
            _user = user;
            _enemy = enemy;
        }
    }
}
