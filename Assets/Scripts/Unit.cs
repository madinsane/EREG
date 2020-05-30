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
        public UnitManager unitManager;
        public int Id { get; protected set; }
        public List<SkillStats> Skills { get; private set; }

        public Unit(UnitStats stats, List<SkillStats> skills)
        {
            Id = stats.Id;
            Stats = stats;
            this.Skills = skills;
            InitResources();
        }

        void InitResources()
        {
            CurrentHealth = Stats.MaxHealth;
            CurrentMana = Stats.MaxMana;
        }

        public void ChangeUnit(UnitStats stats, List<SkillStats> skills)
        {
            Id = stats.Id;
            Stats = stats;
            this.Skills = skills;
            InitResources();
            enabled = true;
        }

        public void ChangeHealth(int value)
        {
            if (CurrentHealth + value > Stats.MaxHealth)
            {
                CurrentHealth = Stats.MaxHealth;
            }
            else
            {
                CurrentHealth += value;
                if (CurrentHealth <= 0)
                {
                    Die();
                }
            }
        }

        public bool CheckCost(int value, Constants.CostTypes skillType)
        {
            if (skillType == Constants.CostTypes.Attack)
            {
                if (CurrentHealth - value < 0)
                {
                    return false;
                }
            } else
            {
                if (CurrentMana - value < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void ApplyCost(int value, Constants.CostTypes skillType)
        {
            if (skillType == Constants.CostTypes.Attack)
            {
                CurrentHealth -= value;
            }
            else
            {
                CurrentMana -= value;
            }
        }

        public void Die()
        {
            enabled = false;
        }
    }
}
