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
        public abstract int Health { get; set; }
        public abstract double Health_X { get; set; }
        public abstract double Resistance_X { get; set; }
        public abstract double Crit_X { get; set; }
        public virtual void Attack1(Entity entity)
        {
            entity.TakeDamage(5);
            Console.WriteLine($"{nameof(Class)} punched {nameof(entity)}");
        }
        public virtual void Attack2(Entity entity)
        {
            entity.TakeDamage(10);
            Console.WriteLine($"{nameof(Class)} kicked {nameof(entity)}");
        }
        public virtual void Special1(Entity entity) { return; }
        public virtual void Special2(Entity entity) { return; }

        public abstract Player NewPlayer();

        public Class()
        {

        }
    }
}
