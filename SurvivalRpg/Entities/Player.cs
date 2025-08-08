using SurvivalRpg.PlayableClasses;
using SurvivalRpg.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalRpg.Entities
{
    public class Player : Entity
    {
        public override int Health { get; set; }
        public override int MaxHealth { get; set; }
        public MapService Map { get; private set; }

        public Class _Class { get; set; }

        public Player(MapService map, Class playerClass)
        {
            Map = map;
            _Class = playerClass;
            Health = playerClass.Health;
            MaxHealth = playerClass.Health;
        }

        public void Attack1(Enemy enemy)
        {
            _Class.Attack1(enemy);
        }

        public void Attack2(Enemy enemy)
        {
            _Class.Attack2(enemy);
        }

        public void Special1(Enemy enemy)
        {
            _Class.Special1(enemy);
        }

        public void Special2(Enemy enemy)
        {
            _Class.Special2(enemy);
        }

        public override string ToString()
        {
            return $"You are a {_Class}";
        }
    }
}
