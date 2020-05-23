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
            public bool hit;
            public int damage;
            public int statusPower;
            public bool isWeak;
            public bool isTechnical;
            public bool removeStatus;
            public Constants.StatusTypes status;
            public Constants.StatusTypes returnStatus;
            public bool isCrit;

            public DamagePacket(bool hit, int power = 0, int statusPower = 0, bool isWeak = false, bool isTechnical = false, bool removeStatus = false, Constants.StatusTypes status = Constants.StatusTypes.None, Constants.StatusTypes returnStatus = Constants.StatusTypes.None, bool isCrit = false) : this()
            {
                this.hit = hit;
                damage = power;
                this.statusPower = statusPower;
                this.isWeak = isWeak;
                this.isTechnical = isTechnical;
                this.removeStatus = removeStatus;
                this.status = status;
                this.returnStatus = returnStatus;
                this.isCrit = isCrit;
            }
        }

        internal static int RandomInt(int min, int max)
        {
            if (random == null)
            {
                random = new System.Random();
            }
            return random.Next(min, max+1);
        }

        static bool TryChance(float outSkillChance, float outUnitChance, float incChance)
        {
            float chance = outSkillChance * (outUnitChance / 100);
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
            float accuracy = skill.Accuracy * ((float)attacker.Accuracy / 100);
            //Always hit if evasion <= 0
            if (defender.Evasion > 0)
            {
                accuracy /= (float)defender.Evasion / 100;
                int evasionRoll = RandomInt(1, 100);
                if (accuracy < evasionRoll)
                {
                    return new DamagePacket(false);
                }
            }
            //Hit
            //Set Type
            float power = 0;
            float defense = 0;
            float resist = 0;
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
                    resist = defender.ResistCold;
                    break;
                case Constants.DamageTypes.Fire:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    resist = defender.ResistFire;
                    break;
                case Constants.DamageTypes.Wind:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    resist = defender.ResistWind;
                    break;
                case Constants.DamageTypes.Arcane:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    resist = defender.ResistArcane;
                    break;
                case Constants.DamageTypes.Psychic:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    resist = defender.ResistPsychic;
                    break;
                case Constants.DamageTypes.Light:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    resist = defender.ResistLight;
                    break;
                case Constants.DamageTypes.Dark:
                    power = attacker.MagicPower;
                    defense = defender.MagicDefense;
                    resist = defender.ResistDark;
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
                float critMulti = attacker.CritMulti * ((float)skill.CritMulti / 100);
                power *= critMulti / 100;
            }
            //Check Status
            int statusChance = skill.StatusChance;
            Constants.StatusTypes status = Constants.StatusTypes.None;
            if (!isTechnical)
            {
                if (statusChance > 0)
                {
                    if (isCrit)
                    {
                        statusChance = (int)(statusChance * Constants.CRITICAL_STATUS_MULTI);
                    }
                    bool statusAttempt;
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
            //Calculate final power
            power *= 100 - resist;
            float statusPower = 0;
            if (status != Constants.StatusTypes.None)
            {
                statusPower = power;
                statusPower *= (float)attacker.StatusPower / 100;
                statusPower *= (float)skill.StatusPower / 100;
            }
            if (defense > 0)
            {
                power /= defense / 100;
            }
            power *= (float)skill.Power / 100;
            DamagePacket packet = new DamagePacket(true, (int)power, (int)statusPower, isWeak, isTechnical, removeStatus, status, returnStatus, isCrit);
            return packet;
        }
    }
}
