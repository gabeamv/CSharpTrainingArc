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

        public override string Name { get; } = "Warrior";
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

        public override void Special1(Enemy enemy)
        {
            enemy.TakeDamage(10);
            Console.WriteLine($"{this} threw {enemy._Breed}.");
        }

        public override void Special2(Enemy enemy)
        {
            enemy.TakeDamage(15);
            Console.WriteLine($"{this} stomped on {enemy._Breed}.");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
