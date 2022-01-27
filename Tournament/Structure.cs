using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourWarServer.Tournament
{
    public struct Structure
    {
        public int ID { get; private set; }
        public float Damage { get; set; }
        public float AttackSpeed { get; set; }
        public float Health { get; set; }

        public Structure(int id , float damage , float attackSpeed , float health)
        {
            ID = id;
            Damage = damage;
            AttackSpeed = attackSpeed;
            Health = health;
        }
    }
}
