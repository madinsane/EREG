﻿using System.Collections;
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
        public string NameStr { get; set; }
        public List<SkillStats> Skills { get; private set; }
        public bool IsPlayer { get; protected set; }
        public List<Effect> Effects { get; protected set; }
        public bool IsDown { get; set; }

        public Unit(UnitStats stats, List<SkillStats> skills)
        {
            Id = stats.Id;
            Stats = stats;
            Skills = skills;
            InitResources();
        }

        void InitResources()
        {
            CurrentHealth = Stats.MaxHealth;
            CurrentMana = Stats.MaxMana;
        }

        public void ChangeUnit(UnitStats stats, List<SkillStats> skills, string nameStr)
        {
            Id = stats.Id;
            Stats = stats;
            Skills = skills;
            NameStr = nameStr;
            IsDown = false;
            InitResources();
            enabled = true;
        }

        public void TakeHit(Damage.DamagePacket hit)
        {
            if (hit.hit)
            {
                if (hit.damage > 0)
                {
                    ChangeHealth(-hit.damage);
                }
                if (hit.status != Constants.StatusTypes.None)
                {
                    AddStatus(hit.status, hit.statusPower);
                }
                if (hit.removeStatus)
                {
                    RemoveStatus();
                }
                if (!IsDown && (hit.isWeak || hit.isCrit))
                {
                    IsDown = true;
                }
            }
        }

        public Constants.StatusTypes GetStatus()
        {
            if (Effects == null)
            {
                Effects = new List<Effect>();
            }
            return Effects.Find(x => x.Type == Constants.EffectType.Status).StatusType;
        }

        public void RemoveStatus()
        {
            if (Effects == null)
            {
                Effects = new List<Effect>();
            }
            Effects.RemoveAll(x => x.Type == Constants.EffectType.Status);
        }

        public void AddStatus(Constants.StatusTypes type, int statusPower)
        {
            if (Effects == null)
            {
                Effects = new List<Effect>();
            }
            RemoveStatus();
            Effects.Add(new Effect(Constants.EffectType.Status, Constants.BuffTypes.None, type, statusPower));
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
            unitManager.UpdateGlobes();
        }

        public virtual void Die()
        {
            enabled = false;
        }
    }
}
