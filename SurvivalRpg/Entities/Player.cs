using SurvivalRpg.IPlayableClasses;
using SurvivalRpg.PlayableClasses;
using SurvivalRpg.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalRpg.Entities
{
    public class Player : Entity
    {
        public override int Health { get; set; }
        public override int MaxHealth { get; set; }
        public MapService Map { get; private set; }

        private PlayerClass _PlayerClass { get; set; }

        public Player(MapService map, PlayerClass playerClass)
        {
            Map = map;
            _PlayerClass = playerClass;
        }

    }
}
