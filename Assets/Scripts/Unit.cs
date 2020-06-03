using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        public float CurrentHealth { get; private set; }
        public float CurrentMana { get; private set; }
        public UnitStats Stats { get; protected set; }
        public UnitManager unitManager;
        public int Id { get; protected set; }
        public string NameStr { get; set; }
        public List<SkillStats> Skills { get; protected set; }
        public bool IsPlayer { get; protected set; }
        public List<Effect> Effects { get; protected set; }
        public bool IsDown { get; set; }
        public int TurnCounter { get; set; }
        public bool OneMore { get; set; }

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

        public virtual void TakeHit(Damage.DamagePacket hit)
        {
            if (hit.hit)
            {
                if (hit.damage > 0)
                {
                    ChangeHealth(-hit.damage);
                }
                if (hit.status != Constants.StatusTypes.None)
                {
                    AddStatus(hit.status, hit.statusPower, hit.statusDuration);
                    Stats.Status = hit.status;
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
            Effect status = Effects.Find(x => x.Type == Constants.EffectType.Status);
            if (status == null)
            {
                return Constants.StatusTypes.None;
            }
            return status.StatusType;
        }

        public float GetStatusPower()
        {
            if (Effects == null)
            {
                Effects = new List<Effect>();
            }
            Effect status = Effects.Find(x => x.Type == Constants.EffectType.Status);
            if (status == null)
            {
                return 0;
            }
            return status.Power;
        }

        public void RemoveStatus()
        {
            if (Effects == null)
            {
                Effects = new List<Effect>();
            }
            Constants.StatusTypes type = GetStatus();
            if (type == Constants.StatusTypes.Berserk || type == Constants.StatusTypes.Confuse)
            {
                if (type == Constants.StatusTypes.Berserk)
                {
                    Stats.AttackDefense *= Constants.BERSERK_MODIFIER;
                    Stats.AttackPower *= Constants.BERSERK_MODIFIER;
                    Stats.MagicDefense *= Constants.BERSERK_MODIFIER;
                    unitManager.log.Add("Berserk wears off " + NameStr);
                }
                else
                {
                    Stats.Accuracy *= Constants.CONFUSE_MODIFIER;
                    Stats.Evasion *= Constants.CONFUSE_MODIFIER;
                    unitManager.log.Add("Confuse wears off " + NameStr);
                }
            }
            Effects.RemoveAll(x => x.Type == Constants.EffectType.Status);
            if (Stats == null)
            {
                return;
            }
            Stats.Status = Constants.StatusTypes.None;
        }

        public void ClearEffects()
        {
            if (Effects == null)
            {
                Effects = new List<Effect>();
            }
            Effects.Clear();
            if (Stats == null)
            {
                return;
            }
            Stats.Status = Constants.StatusTypes.None;
        }

        public virtual void AddStatus(Constants.StatusTypes type, int statusPower, int duration = 2)
        {
            if (Effects == null)
            {
                Effects = new List<Effect>();
            }
            RemoveStatus();
            Effects.Add(new Effect(Constants.EffectType.Status, Constants.BuffTypes.None, type, statusPower, duration));
            if (type == Constants.StatusTypes.Berserk || type == Constants.StatusTypes.Confuse)
            {
                if (type == Constants.StatusTypes.Berserk)
                {
                    Stats.AttackDefense /= Constants.BERSERK_MODIFIER;
                    Stats.AttackPower /= Constants.BERSERK_MODIFIER;
                    Stats.MagicDefense /= Constants.BERSERK_MODIFIER;
                    unitManager.log.Add(NameStr + " Berserk");
                } else
                {
                    Stats.Accuracy /= Constants.CONFUSE_MODIFIER;
                    Stats.Evasion /= Constants.CONFUSE_MODIFIER;
                    unitManager.log.Add(NameStr + " are Confused");
                }
            }
        }

        public virtual void ApplyDuration()
        {
            if (Effects == null)
            {
                return;
            }
            foreach (Effect effect in Effects)
            {
                effect.Duration--;
            }
            Constants.StatusTypes type = GetStatus();
            if (type == Constants.StatusTypes.Berserk || type == Constants.StatusTypes.Confuse)
            {
                if (type == Constants.StatusTypes.Berserk)
                {
                    Stats.AttackDefense *= Constants.BERSERK_MODIFIER;
                    Stats.AttackPower *= Constants.BERSERK_MODIFIER;
                    Stats.MagicDefense *= Constants.BERSERK_MODIFIER;
                    unitManager.log.Add("Berserk wears off " + NameStr);
                }
                else
                {
                    Stats.Accuracy *= Constants.CONFUSE_MODIFIER;
                    Stats.Evasion *= Constants.CONFUSE_MODIFIER;
                    unitManager.log.Add("Confuse wears off " + NameStr);
                }
            }
            Effects.RemoveAll(x => x.Duration <= 0);
            Constants.StatusTypes status = GetStatus();
            Stats.Status = status;
        }

        public void ApplyBurn()
        {
            ChangeHealth(-(int)GetStatusPower());
        }

        public int ApplyBlast()
        {
            int damage = (int)GetStatusPower();
            ChangeHealth(-damage);
            return damage;
        }

        public void ApplyCurse(int multiplier)
        {
            float power = GetStatusPower();
            power = Constants.CURSE_COST * ((100 + power) / 100);
            ChangeHealth(multiplier * (int)(CurrentHealth * (power / 100)));
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
                    unitManager.log.Add(NameStr + " dies");
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
                if (CurrentMana - value > Stats.MaxMana)
                {
                    CurrentMana = Stats.MaxMana;
                }
                else
                {
                    CurrentMana -= value;
                }
            }
            unitManager.UpdateGlobes();
        }

        public virtual void Die()
        {
            enabled = false;
        }
    }
}
