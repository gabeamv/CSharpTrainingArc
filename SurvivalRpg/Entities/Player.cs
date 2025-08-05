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
    public abstract class Player : Entity
    {
        public override int Health { get; set; } = 50;
        public override int MaxHealth { get; set; } = 50;
        public MapService Map { get; private set; }
        public MapService.Coord PlayerCoords { get; private set; }
        
        public Player(MapService map, MapService.Coord playerCoords)
        {
            Map = map;
            PlayerCoords = playerCoords;
        }

    }
}
