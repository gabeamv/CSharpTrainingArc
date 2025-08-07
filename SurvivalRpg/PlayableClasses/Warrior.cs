using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurvivalRpg.Entities;
using SurvivalRpg.Services;
using static SurvivalRpg.Services.MapService;
using System.Runtime.CompilerServices;

namespace SurvivalRpg.PlayableClasses
{
    public class Warrior : Class
    {

        public override int Health { get; set; } = 75;
        public override double Health_X { get; set; } = 1.5;
        public override double Resistance_X { get; set; } = 2;
        public override double Crit_X { get; set; } = 1;

        // TODO: In the future, maybe implement a list of possible moves that a warrior can store, through config file.
        public Warrior() : base()
        {
            
        }

        public override Player NewPlayer()
        {
            return new Player(new MapService(), this);
        }

        public override void Special1(Entity entity)
        {
            entity.TakeDamage(10);
            Console.WriteLine($"{this} threw {entity.GetType()}.");
        }

        public override void Special2(Entity entity)
        {
            entity.TakeDamage(15);
            Console.WriteLine($"{this} stomped on {entity.GetType()}.");
        }
    }
}
