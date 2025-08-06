using SurvivalRpg.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurvivalRpg.Services;

namespace SurvivalRpg.PlayableClasses
{
    public class Ranger : Class
    {
        public override int Health { get; set; } = 50;
        public override double Health_X { get; set; } = 1.5;
        public override double Resistance_X { get; set; } = 1;
        public override double Crit_X { get; set; } = 1.5;

        public Ranger() : base() 
        {
            
        }
        public override Player NewPlayer()
        {
            return new Player(new MapService(), this);
        }

        public override void Special1(Entity entity)
        {
            entity.TakeDamage(20);
            Console.WriteLine($"{nameof(Class)} shot {nameof(entity)}.");
        }

        public override void Special2(Entity entity)
        {
            entity.TakeDamage(35);
            Console.WriteLine($"{nameof(Class)} barraged {nameof(entity)} with arrows.");
        }
    }
}
