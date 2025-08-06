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
        public int Health { get; set; }
        public abstract void Attack1();
        public abstract void Attack2();
        public abstract void Special1();
        public abstract void Special2();

        public abstract Player NewPlayer();

        public Class()
        {

        }
    }
}
