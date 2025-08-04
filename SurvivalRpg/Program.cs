using SurvivalRpg.Entities;
using SurvivalRpg.PlayableClasses;
using SurvivalRpg.Services;
using System.Drawing;

namespace SurvivalRpg
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Entity> entities = new List<Entity>();
            MapService.Coord playerCoords = new MapService.Coord(0,0);
            MapService userMap = new MapService(entities, ref playerCoords);
            Player user = new Player(userMap, playerCoords);

            while (true)
            {
                Console.WriteLine(user.Map.MapSeen());
                Console.WriteLine("1. North\n2. South\n3. East\n 4. West");
                string direction = Console.ReadLine();
                int dir;
                if (int.TryParse(direction, out dir))
                {
                    if (dir == 1) userMap.GoNorth();
                    if (dir == 2) userMap.GoSouth();
                    if (dir == 3) userMap.GoEast();
                    if (dir == 4) userMap.GoWest();
                }
            }
        }

        public void GetDirection()
        {

        }
    }

}
