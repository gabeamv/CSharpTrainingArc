using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalRpg.Entities
{
    public abstract class Entity
    {
        public abstract int Health { get; set; }
        public abstract int MaxHealth { get; set; }
        
        public virtual void Heal(int healAmount)
        {
            Health += healAmount;
            if (Health > MaxHealth) Health = MaxHealth;
        }

        public virtual bool IsAlive() => Health > 0;

        public virtual void TakeDamage(int damageAmount)
        {
            Health -= damageAmount;
        }
    }
}
