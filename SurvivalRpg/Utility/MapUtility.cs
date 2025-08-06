using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalRpg.Utility
{
    public static class MapUtility
    {
        public enum MapSymbol
        {
            NOT_SEEN = '*',
            SEEN = 'X',
            PLAYER = 'P',
            ENCOUNTER = 'E',
            CONSUMABLE = 'C',
            DUNGEON_MAP = 'M',
            ENTRANCE = 'S'
        }

        public const string NORTH = "W";
        public const string SOUTH = "S";
        public const string EAST = "D";
        public const string WEST = "A";
    }
}
