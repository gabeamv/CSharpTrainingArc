using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalRpg.Entities
{
    public class Enemy : Entity
    {
        public override int Health { get; set; }
        public override int MaxHealth { get; set; }
        public Enemy()
        {

        }
        
    }
}
