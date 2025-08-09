using SurvivalRpg.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalRpg.PlayableClasses
{
    public abstract class Class
    {
        public abstract string Name { get; }
        public abstract int Health { get; set; }
        public abstract double Health_X { get; set; }
        public abstract double Resistance_X { get; set; }
        public abstract double Crit_X { get; set; }
        public virtual void Attack1(Enemy enemy)
        {
            enemy.TakeDamage(5);
            Console.WriteLine($"{this} punched {enemy._Breed}");
        }
        public virtual void Attack2(Enemy enemy)
        {
            enemy.TakeDamage(10);
            Console.WriteLine($"{this} kicked {enemy._Breed}");
        }
        public virtual void Special1(Enemy enemy) { return; }
        public virtual void Special2(Enemy enemy) { return; }

        public abstract Player NewPlayer();

        public Class()
        {

        }
    }
}
