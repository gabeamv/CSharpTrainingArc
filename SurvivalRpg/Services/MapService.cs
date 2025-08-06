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
        public const int MAX_DIM_INDEX = 9;
        public const int MIN_DIM_INDEX = 0;
        public const int MAP_SIZE = 100;
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

        public bool[,] Seen { get; set; } = new bool[MAX_DIM_INDEX + 1, MAX_DIM_INDEX + 1];
        public MapUtility.MapSymbol[,] Map { get; set; } = new MapUtility.MapSymbol[MAX_DIM_INDEX + 1, MAX_DIM_INDEX + 1];
        public Coord PlayerCoord;

        public MapService()
        {
            GenerateMap(); // Initializes PlayerCoord
        }

        public string MapSeen()
        {
            StringBuilder seenMap = new StringBuilder();
            for (int y = 0; y < Seen.GetLength(0); y++)
            {
                for (int x = 0; x < Seen.GetLength(1); x++)
                {
                    if (!Seen[x, y]) seenMap.Append((char)MapUtility.MapSymbol.NOT_SEEN + "   ");
                    else if (PlayerCoord.X == x && PlayerCoord.Y == y) seenMap.Append((char)MapUtility.MapSymbol.PLAYER + "   ");
                    else if (Seen[x, y] && Map[x, y] == MapUtility.MapSymbol.NOT_SEEN) seenMap.Append((char)MapUtility.MapSymbol.SEEN + "   ");
                    else seenMap.Append((char)Map[x, y] + "   ");
                    if (x == Seen.GetLength(1) - 1) seenMap.Append("\n");
                }
            }
            return seenMap.ToString();
        }

        public string FullMap()
        {
            StringBuilder fullMap = new StringBuilder();
            for (int y = 0; y < Map.GetLength(0); y++)
            {
                for (int x = 0; x < Map.GetLength(1); x++)
                {
                    fullMap.Append(((char)Map[x, y]).ToString() + "   ");
                    if (x == MAX_DIM_INDEX) fullMap.Append("\n");
                }
            }
            return fullMap.ToString();
        }

        public void GoNorth()
        {
            if (PlayerCoord.Y - 1 < 0) throw new IndexOutOfRangeException();
            Seen[PlayerCoord.X, --PlayerCoord.Y] = true;
        }

        public void GoSouth()
        {
            if (PlayerCoord.Y + 1 > MAX_DIM_INDEX) throw new IndexOutOfRangeException();
            Seen[PlayerCoord.X, ++PlayerCoord.Y] = true;
        }

        public void GoEast()
        {
            if (PlayerCoord.X + 1 > MAX_DIM_INDEX) throw new IndexOutOfRangeException();
            Seen[++PlayerCoord.X, PlayerCoord.Y] = true;
        }

        public void GoWest()
        {
            if (PlayerCoord.X - 1 < 0) throw new IndexOutOfRangeException();
            Seen[--PlayerCoord.X, PlayerCoord.Y] = true;
        }

        public void DisplayMapSeen()
        {
            Console.WriteLine(MapSeen());
        }

        public void DisplayMap()
        {
            Console.WriteLine(FullMap());
        }

        public MapUtility.MapSymbol GetEvent()
        {
            return Map[PlayerCoord.X, PlayerCoord.Y];
        }

        public void GenerateMap()
        {
            int ENCOUNTER_INSTANCES = 20;
            int CONSUMABLE_INSTANCES = 10;
            int ENTRANCE_INSTANCES = 2;
            int DUNGEON_MAP_INSTANCES = 1;
            int NOT_SEEN_INSTANCES = MAP_SIZE - ENCOUNTER_INSTANCES - CONSUMABLE_INSTANCES - ENTRANCE_INSTANCES
                - DUNGEON_MAP_INSTANCES;

            // Create a List<(int, int)> to store coordinates 0,0 to 9,9
            List<(int x, int y)> coords = new List<(int x, int y)>();
            for (int i = 0; i < MAX_DIM_INDEX + 1; i++)
            {
                for (int j = 0; j < MAX_DIM_INDEX + 1; j++)
                {
                    coords.Add((i, j));
                }
            }

            // Get a randomly chosen point from the first and last columns of the map
            int s1x = MIN_DIM_INDEX;
            int s1y = random.Next(0, MAX_DIM_INDEX + 1);
            // Randomly chosen start is the player's initial coordinates.
            PlayerCoord.X = s1x;
            PlayerCoord.Y = s1y;
            int s2x = MAX_DIM_INDEX;
            int s2y = random.Next(0, MAX_DIM_INDEX + 1);

            // Set s1.
            for (int i = 0; i < coords.Count; i++) 
            {
                if (coords[i] == (s1x, s1y))
                {
                    Map[s1x, s1y] = MapUtility.MapSymbol.ENTRANCE;
                    coords.RemoveAt(i);
                    break;
                }
            }

            // Set s2.
            for (int i = 0; i < coords.Count; i++)
            {
                if (coords[i] == (s2x, s2y))
                {
                    Map[s2x, s2y] = MapUtility.MapSymbol.ENTRANCE;
                    coords.RemoveAt(i);
                    break;
                }
            }
            SetSymbolAtCoord(coords, NOT_SEEN_INSTANCES, MapUtility.MapSymbol.NOT_SEEN);
            SetSymbolAtCoord(coords, DUNGEON_MAP_INSTANCES, MapUtility.MapSymbol.DUNGEON_MAP);
            SetSymbolAtCoord(coords, ENCOUNTER_INSTANCES, MapUtility.MapSymbol.ENCOUNTER);
            SetSymbolAtCoord(coords, CONSUMABLE_INSTANCES, MapUtility.MapSymbol.CONSUMABLE);

            for (int x = 0; x < MAX_DIM_INDEX + 1; x++)
            {
                for (int j = 0; j < MAX_DIM_INDEX + 1; j++)
                {
                    Seen[x, j] = false;
                }
            }
            Seen[PlayerCoord.X, PlayerCoord.Y] = true;
        }


        private void SetSymbolAtCoord(List<(int, int)> coords, int numInstances, 
            MapUtility.MapSymbol symbol)
        {
            for (int i = 0; i < numInstances; i++) // get n instances of randomly chosen coords
            {
                int randIdx = random.Next(0, coords.Count);
                (int x, int y) randCoord = coords[randIdx];
                coords.RemoveAt(randIdx);
                Map[randCoord.x, randCoord.y] = symbol;
            }
        }

        
    }
}
