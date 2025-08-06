using SurvivalRpg.Entities;
using SurvivalRpg.Game;
using SurvivalRpg.PlayableClasses;
using SurvivalRpg.Services;
using SurvivalRpg.Utility;
using System.Diagnostics;
using System.Drawing;

namespace SurvivalRpg
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Entity> entities = new List<Entity>(); // list of entities.
            MapService.Coord playerCoords = new MapService.Coord(0,0);
            
            Player user = new Warrior(new MapService());

            while (true) // game loop
            {
                user.Map.DisplayMapSeen();
                //user.Map.DisplayMap();
                Console.WriteLine("W. North\nS. South\nD. East\nA. West");
                string strDir = Console.ReadLine();
                while (strDir.ToUpper() != MapUtility.NORTH && strDir.ToUpper() != MapUtility.SOUTH && 
                    strDir.ToUpper() != MapUtility.EAST && strDir.ToUpper() != MapUtility.WEST)
                {
                    Console.WriteLine("Please Enter Valid Input:\nW. North\nS. South\nD. East\n . West");
                    strDir = Console.ReadLine();
                }

                try
                {
                    switch (strDir.ToUpper())
                    {
                        case MapUtility.NORTH:
                            user.Map.GoNorth();
                            break;
                        case MapUtility.SOUTH:
                            user.Map.GoSouth();
                            break;
                        case MapUtility.EAST:
                            user.Map.GoEast();
                            break;
                        case MapUtility.WEST:
                            user.Map.GoWest();
                            break;
                        default: break;
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    Console.WriteLine("Cannot Go There!");
                }
                
              
                switch (user.Map.GetEvent())
                {
                    case MapUtility.MapSymbol.ENCOUNTER:
                        // Encounter
                        Console.WriteLine("You have encountered a monster.");
                        Encounter encounter = new Encounter(user, new Enemy());
                        break;
                    case MapUtility.MapSymbol.CONSUMABLE:
                        // give consumable item.
                        Console.WriteLine("You found an item.");
                        break;
                    case MapUtility.MapSymbol.DUNGEON_MAP:
                        Console.WriteLine("You found a dungeon map.");
                        break;
                    case MapUtility.MapSymbol.ENTRANCE:
                        Console.WriteLine("Entering new area...");
                        user.Map.GenerateMap();
                        break;
                    // reveal the entire dungeon. 
                    default: break; // Do nothing
                }

                
            }
        }

    }

}
