using SurvivalRpg.Breeds;
using SurvivalRpg.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalRpg.Breeds
{
    public class Zombie : Breed
    {
        public override int Health { get; set; } = 20;
        public override double Health_X { get; set; }
        public override double Resistance_X { get; set; }
        public override double Crit_X { get; set; }

        public Zombie()
        {

        }
        
        public override Enemy NewEnemy()
        {
            return new Enemy(this);
        }

        public override void Special1(Entity entity)
        {
            entity.TakeDamage(5);
            Console.WriteLine($"{nameof(Zombie)} scratched {nameof(entity)}");
        }

        public override void Special2(Entity entity)
        {
            entity.TakeDamage(15);
            Console.WriteLine($"{nameof(Zombie)} bit {nameof(entity)}");
        }
    }
}
