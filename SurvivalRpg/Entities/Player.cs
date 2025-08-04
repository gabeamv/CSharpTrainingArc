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
        public MapService Map { get; private set; }
        public Class PlayerClass { get; private set; }
        public MapService.Coord PlayerCoords { get; private set; }
        public Player(MapService map, MapService.Coord playerCoords)
        {
            Map = map;
            PlayerCoords = playerCoords;

        }
        public override int Health 
        { 
            get => throw new NotImplementedException(); set => throw new NotImplementedException(); 
        }
        public override int MaxHealth 
        { 
            get => throw new NotImplementedException(); set => throw new NotImplementedException(); 
        }
    }
}
