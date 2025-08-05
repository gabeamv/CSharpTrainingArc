using SurvivalRpg.IPlayableClasses;
using SurvivalRpg.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurvivalRpg.Services;

namespace SurvivalRpg.PlayableClasses
{
    internal class Rogue : Player, IRogue
    {
        public Rogue(MapService map, MapService.Coord coord) : base(map, coord) { }
        
    }
}
