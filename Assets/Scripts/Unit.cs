using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        public int CurrentHealth { get; private set; }
        public int CurrentMana { get; private set; }
        public UnitStats Stats { get; private set; }
        void Awake()
        {
            Stats = new UnitStats();
            GetStatsByID(0);
            InitStats();

        }

        void GetStatsByID(int id)
        {

        }

        void InitStats()
        {
            CurrentHealth = Stats.MaxHealth;
            CurrentMana = Stats.MaxMana;
        }
    }
}
