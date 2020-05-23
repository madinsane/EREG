using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Damage
    {
        static System.Random random;

        internal struct DamagePacket
        {
            int damage;
            bool isWeak;
            bool isTechnical;
            bool removeStatus;
            Constants.StatusTypes status;
            Constants.StatusTypes returnStatus;
            bool isCrit;
        }

        internal static int RandomInt(int min, int max)
        {
            if (random == null)
            {
                random = new System.Random();
            }
            return random.Next(min, max+1);
        }

        static bool TryChance(int outSkillChance, int outUnitChance, int incChance)
        {
            int chance = outSkillChance * (outUnitChance / 100);
            chance *= incChance / 100;
            int roll = RandomInt(1, 100);
            if (chance >= roll)
            {
                return true;
            }
            return false;
        }

        internal static DamagePacket Hit(SkillStats skill, UnitStats attacker, UnitStats defender)
        {
            //Check evasion
            int accuracy = skill.Accuracy * (attacker.Accuracy / 100);
            if (defender.Evasion <= 0)
            {
                accuracy = 0;
            }
            else
            {
                accuracy /= (defender.Evasion / 100);
            }
            int evasionRoll = RandomInt(1, 100);
            if (accuracy < evasionRoll)
            {
                //miss
            }
            //Hit
            //Set Type
            int power = 0;
            int defense = 0;
            int resist = 0;
            switch (skill.DamageType)
            {
                case Constants.DamageTypes.Physical:
                    power = attacker.AttackPower;
                    defense = defender.AttackDefense;
                    resist = defender.ResistPhysical;
                    break;
                case Constants.DamageTypes.Projectile:
                    power = attacker.AttackPower;
                    defense = defender.AttackDefense;
                    resist = defender.ResistProjectile;
                    break;
                case Constants.DamageTypes.Almighty:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    break;
                case Constants.DamageTypes.Electric:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    resist = defender.ResistElectric;
                    break;
                case Constants.DamageTypes.Cold:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    resist = defender.ResistElectric;
                    break;
                case Constants.DamageTypes.Fire:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    resist = defender.ResistElectric;
                    break;
                case Constants.DamageTypes.Wind:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    resist = defender.ResistElectric;
                    break;
                case Constants.DamageTypes.Arcane:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    resist = defender.ResistElectric;
                    break;
                case Constants.DamageTypes.Psychic:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    resist = defender.ResistElectric;
                    break;
                case Constants.DamageTypes.Light:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    resist = defender.ResistElectric;
                    break;
                case Constants.DamageTypes.Dark:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    resist = defender.ResistElectric;
                    break;
            }
            //Check Weakness
            bool isWeak = false;
            if (resist <= -50)
            {
                isWeak = true;
            }
            //Check Technical
            bool isTechnical = false;
            bool removeStatus = false;
            Constants.StatusTypes returnStatus = Constants.StatusTypes.None;
            switch (defender.Status)
            {
                case Constants.StatusTypes.None:
                    break;
                case Constants.StatusTypes.Shock:
                    if (skill.DamageType == Constants.DamageTypes.Physical)
                    {
                        power = (int)(power * Constants.TECHNICAL_MULTI);
                        isTechnical = true;
                        removeStatus = true;
                        returnStatus = Constants.StatusTypes.Shock;
                    } else if (skill.DamageType == Constants.DamageTypes.Arcane)
                    {
                        power = (int)(power * Constants.TECHNICAL_MULTI);
                        isTechnical = true;
                        removeStatus = true;
                    }
                    break;
                case Constants.StatusTypes.Freeze:
                    if (skill.DamageType == Constants.DamageTypes.Physical || skill.DamageType == Constants.DamageTypes.Projectile)
                    {
                        power = (int)(power * Constants.TECHNICAL_MULTI);
                        isTechnical = true;
                        removeStatus = true;
                    } else if (skill.DamageType == Constants.DamageTypes.Arcane)
                    {
                        power = (int)(power * Constants.TECHNICAL_MULTI);
                        isTechnical = true;
                    }
                    break;
                case Constants.StatusTypes.Burn:
                    if (skill.DamageType == Constants.DamageTypes.Arcane)
                    {
                        power = (int)(power * Constants.TECHNICAL_MULTI);
                        isTechnical = true;
                    }
                    break;
                case Constants.StatusTypes.Sleep:
                    power = (int)(power * Constants.TECHNICAL_MULTI);
                    isTechnical = true;
                    break;
                case Constants.StatusTypes.Forget:
                    if (skill.DamageType == Constants.DamageTypes.Psychic)
                    {
                        power = (int)(power * Constants.TECHNICAL_MULTI);
                        isTechnical = true;
                    }
                    break;
                case Constants.StatusTypes.Berserk:
                    if (skill.DamageType == Constants.DamageTypes.Psychic)
                    {
                        power = (int)(power * Constants.TECHNICAL_MULTI);
                        isTechnical = true;
                    }
                    break;
                case Constants.StatusTypes.Confuse:
                    if (skill.DamageType == Constants.DamageTypes.Psychic)
                    {
                        power = (int)(power * Constants.TECHNICAL_MULTI);
                        isTechnical = true;
                    }
                    break;
                case Constants.StatusTypes.Brainwash:
                    if (skill.DamageType == Constants.DamageTypes.Psychic)
                    {
                        power = (int)(power * Constants.TECHNICAL_MULTI);
                        isTechnical = true;
                    }
                    break;
                case Constants.StatusTypes.Fear:
                    if (skill.DamageType == Constants.DamageTypes.Psychic)
                    {
                        power = (int)(power * Constants.TECHNICAL_MULTI);
                        isTechnical = true;
                    }
                    break;
            }
            //Check Crit
            bool isCrit = false;
            if (TryChance(skill.CritChance, attacker.CritChance, defender.IncCritChance))
            {
                isCrit = true;
                int critMulti = attacker.CritMulti * (skill.CritMulti / 100);
                power *= critMulti / 100;
            }
            //Check Status
            int statusChance = skill.StatusChance;
            bool statusAttempt = false;
            Constants.StatusTypes status = Constants.StatusTypes.None;
            if (!isTechnical)
            {
                if (statusChance > 0)
                {
                    if (isCrit)
                    {
                        statusChance = (int)(statusChance * Constants.CRITICAL_STATUS_MULTI);
                    }
                    if (skill.StatusType >= Constants.StatusTypes.Sleep)
                    {
                        statusAttempt = TryChance(statusChance, attacker.MentalStatusChance, defender.IncMentalStatus);
                    } else
                    {
                        statusAttempt = TryChance(statusChance, attacker.TypeStatusChance, defender.IncTypeStatus);
                    }
                    if (statusAttempt)
                    {
                        status = skill.StatusType;
                    }
                }
            }
            return new DamagePacket();
        }
    }
}
