using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurvivalRpg.IPlayableClasses;
using SurvivalRpg.Entities;
using SurvivalRpg.Services;
using static SurvivalRpg.Services.MapService;

namespace SurvivalRpg.PlayableClasses
{
    public class Warrior : Player, IWarrior
    {
        public Warrior(MapService map, Coord coords) : base(map, coords) { }
    }
}
