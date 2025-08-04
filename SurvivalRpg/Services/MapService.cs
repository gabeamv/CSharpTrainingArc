using SurvivalRpg.Entities;
using SurvivalRpg.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalRpg.Services
{

    // Service to represent and store positions of entities on the map.
    public class MapService
    {
        private static Random random = new Random();

        // Structure to represent coordinates in map.
        public struct Coord
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Coord(int x, int y)
            {
                X = x; Y = y;
            }
            public override string ToString() { return $"Coords: X = {X}, Y = {Y}"; }

        }

        public bool[,] Seen { get; set; } = new bool[10, 10];
        public char[,] Map { get; set; } = new char[10, 10];
        private List<Entity> EntityCoord;
        private Coord PlayerCoord;

        public MapService(List<Entity> entityCoord, ref Coord playerCoord)
        {
            EntityCoord = entityCoord;
            PlayerCoord = playerCoord;
            Seen[PlayerCoord.X, PlayerCoord.Y] = true;
        }

        public string MapSeen()
        {
            StringBuilder seenMap = new StringBuilder();
            for (int y = 0; y < Seen.GetLength(0); y++)
            {
                for (int x = 0; x < Seen.GetLength(1); x++)
                {
                    if (!Seen[x, y]) seenMap.Append(Util.NOT_SEEN + "  ");
                    else if (PlayerCoord.X == x && PlayerCoord.Y == y) seenMap.Append(Util.PLAYER + "  ");
                    else seenMap.Append(Util.SEEN + "  ");
                    if (x == Seen.GetLength(1) - 1) seenMap.Append("\n");
                }
            }
            return seenMap.ToString();
        }

        public void GoNorth()
        {
            Seen[PlayerCoord.X, --PlayerCoord.Y] = true;
        }

        public void GoSouth()
        {
            Seen[PlayerCoord.X, ++PlayerCoord.Y] = true;
        }

        public void GoEast()
        {
            Seen[++PlayerCoord.X, PlayerCoord.Y] = true;
        }

        public void GoWest()
        {
            Seen[--PlayerCoord.X, PlayerCoord.Y] = true;
        }
    }
}
