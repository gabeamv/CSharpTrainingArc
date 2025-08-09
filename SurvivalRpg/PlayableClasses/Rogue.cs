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
        public override string Name { get; }
        public override int Health { get; set; } = 35;
        public override double Health_X { get; set; } = 1;
        public override double Resistance_X { get; set; } = 1.5;
        public override double Crit_X { get; set; } = 2;

        public Rogue() : base()
        {

        }

        public override void Special1(Enemy enemy)
        {
            enemy.TakeDamage(20);
            Console.WriteLine($"{this} stabbed {enemy._Breed}.");
        }

        public override void Special2(Enemy enemy)
        {
            enemy.TakeDamage(50);
            Console.WriteLine($"{this} cut {enemy._Breed} up.");
        }

        public override Player NewPlayer()
        {
            return new Player(new MapService(), this);
        }
    }
}
