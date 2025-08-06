using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurvivalRpg.Breeds;

namespace SurvivalRpg.Entities
{
    public class Enemy : Entity
    {
        public override int Health { get; set; }
        public override int MaxHealth { get; set; }
        public Breed _Breed { get; set; }
        public Enemy(Breed breed)
        {
            _Breed = breed;
            Health = breed.Health;
            MaxHealth = breed.Health;
        }

        public void Special1(Entity entity)
        {
            _Breed.Special1(entity);
        }

        public void Special2(Entity entity)
        {
            _Breed.Special2(entity);
        }

        public override string ToString()
        {
            return $"{nameof(Enemy)} is {nameof(Breed)}";
        }
        
    }
}
