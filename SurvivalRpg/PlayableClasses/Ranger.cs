using SurvivalRpg.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurvivalRpg.Services;
using System.Reflection.Metadata.Ecma335;

namespace SurvivalRpg.PlayableClasses
{
    public class Ranger : Class
    {
        public override string Name { get; }
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

        public override void Special1(Enemy enemy)
        {
            enemy.TakeDamage(20);
            Console.WriteLine($"{this} shot {enemy._Breed}.");
        }

        public override void Special2(Enemy enemy)
        {
            enemy.TakeDamage(35);
            Console.WriteLine($"{this} barraged {enemy._Breed} with arrows.");
        }
    }
}
