using SurvivalRpg.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalRpg.Breeds
{
     public abstract class Breed
    {
        public abstract int Health { get; set; }
        public abstract double Health_X { get; set; }
        public abstract double Resistance_X { get; set; }
        public abstract double Crit_X { get; set; }
        public abstract void Special1(Entity entity);
        public abstract void Special2(Entity entity);
        public abstract Enemy NewEnemy();
    }
}
