using SurvivalRpg.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurvivalRpg.Services;

namespace SurvivalRpg.PlayableClasses
{
    internal class Rogue : Class
    {
        public override int Health { get; set; } = 35;
        public override double Health_X { get; set; } = 1;
        public override double Resistance_X { get; set; } = 1.5;
        public override double Crit_X { get; set; } = 2;

        public Rogue() : base()
        {

        }

        public override void Special1(Entity entity)
        {
            entity.TakeDamage(20);
            Console.WriteLine($"{nameof(Class)} stabbed {nameof(entity)}.");
        }

        public override void Special2(Entity entity)
        {
            entity.TakeDamage(50);
            Console.WriteLine($"{nameof(Class)} cut {nameof(entity)} up.");
        }

        public override Player NewPlayer()
        {
            return new Player(new MapService(), this);
        }
    }
}
