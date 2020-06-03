using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Damage
    {
        static System.Random random;

        public struct DamagePacket
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
            public int statusDuration;

            public DamagePacket(bool hit, int power = 0, int statusPower = 0, bool isWeak = false, bool isTechnical = false, bool removeStatus = false, Constants.StatusTypes status = Constants.StatusTypes.None, Constants.StatusTypes returnStatus = Constants.StatusTypes.None, bool isCrit = false, int statusDuration = 2) : this()
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
                this.statusDuration = statusDuration;
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

        static bool TryChance(float outSkillChance, float outUnitChance, float incChance, bool isPlayer)
        {
            float chance = outSkillChance * (outUnitChance / 100);
            chance *= incChance / 100;
            int roll = RandomInt(1, 100);
            int roll2 = RandomInt(1, 100);
            //Lucky
            if (ExperimentControl.active)
            {
                if (roll2 < roll)
                {
                    roll = roll2;
                }
            }
            if (chance >= roll)
            {
                return true;
            }
            return false;
        }

        internal static int Heal(SkillStats skill, UnitStats caster)
        {
            float power = caster.MagicPower;
            power *= (float)skill.Power / 100;
            return (int)power;
        }

        internal static KeyValuePair<bool, int> Status(SkillStats skill, UnitStats attacker, UnitStats defender)
        {
            bool status = false;
            bool playerAttacker = false;
            bool playerDefender = false;
            if (attacker.Id == 0)
            {
                playerAttacker = true;
            } else if (defender.Id == 0)
            {
                playerDefender = true;
            }
            if (skill.StatusType == Constants.StatusTypes.Blast)
            {
                float power = attacker.MagicPower;
                power *= (float)attacker.StatusPower / 100;
                power *= (float)skill.StatusPower / 100;
                return new KeyValuePair<bool, int>(true, (int)power);
            }
            //Check evasion
            float accuracy = skill.Accuracy * ((float)attacker.Accuracy / 100);
            //Always hit if evasion <= 0
            if (defender.Evasion > 0)
            {
                accuracy /= (float)defender.Evasion / 100;
                int evasionRoll = RandomInt(1, 100);
                if (ExperimentControl.active)
                {
                    if (playerDefender)
                    {
                        int evasionRoll2 = RandomInt(1, 100);
                        if (evasionRoll2 > evasionRoll)
                        {
                            evasionRoll = evasionRoll2;
                        }
                    }
                }
                if (accuracy < evasionRoll)
                {
                    return new KeyValuePair<bool, int> (false, 0);
                }
            }
            //Check Status
            float statusChance = skill.StatusChance;
            int statusDuration = 2;
            if (statusChance > 0)
            {
                bool statusAttempt;
                if (skill.StatusType >= Constants.StatusTypes.Sleep)
                {
                    statusAttempt = TryChance(statusChance, attacker.MentalStatusChance, defender.IncMentalStatus, playerAttacker);
                    statusDuration = 4;
                }
                else
                {
                    statusAttempt = TryChance(statusChance, attacker.TypeStatusChance, defender.IncTypeStatus, playerAttacker);
                }
                if (statusAttempt)
                {
                    status = true;
                }
            }
            return new KeyValuePair<bool, int>(status, statusDuration);
        }

        internal static DamagePacket Hit(SkillStats skill, UnitStats attacker, UnitStats defender)
        {
            bool playerAttacker = false;
            bool playerDefender = false;
            if (attacker.Id == 0)
            {
                playerAttacker = true;
            }
            else if (defender.Id == 0)
            {
                playerDefender = true;
            }
            //Check evasion
            float accuracy = skill.Accuracy * ((float)attacker.Accuracy / 100);
            //Always hit if evasion <= 0
            if (defender.Evasion > 0)
            {
                accuracy /= (float)defender.Evasion / 100;
                int evasionRoll = RandomInt(1, 100);
                if (ExperimentControl.active)
                {
                    if (playerDefender)
                    {
                        int evasionRoll2 = RandomInt(1, 100);
                        if (evasionRoll2 > evasionRoll)
                        {
                            evasionRoll = evasionRoll2;
                        }
                    }
                }
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
            int statusDuration = 2;
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
                    statusDuration = 4;
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
                    statusDuration = 4;
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
            if (TryChance(skill.CritChance, attacker.CritChance, defender.IncCritChance, playerAttacker))
            {
                isCrit = true;
                float critMulti = attacker.CritMulti * ((float)skill.CritMulti / 100);
                power *= critMulti / 100;
            }
            //Check Status
            float statusChance = skill.StatusChance;
            statusChance *= (100 - resist) / 100;
            Constants.StatusTypes status = Constants.StatusTypes.None;
            if (!isTechnical)
            {
                if (statusChance > 0)
                {
                    if (isCrit)
                    {
                        statusChance *= Constants.CRITICAL_STATUS_MULTI;
                    }
                    bool statusAttempt;
                    if (skill.StatusType >= Constants.StatusTypes.Sleep)
                    {
                        statusAttempt = TryChance(statusChance, attacker.MentalStatusChance, defender.IncMentalStatus, playerAttacker);
                    } else
                    {
                        statusAttempt = TryChance(statusChance, attacker.TypeStatusChance, defender.IncTypeStatus, playerAttacker);
                    }
                    if (statusAttempt)
                    {
                        status = skill.StatusType;
                    }
                }
            }
            //Calculate final power
            power *= (100 - resist) / 100;
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
            DamagePacket packet = new DamagePacket(true, (int)power, (int)statusPower, isWeak, isTechnical, removeStatus, status, returnStatus, isCrit, statusDuration);
            return packet;
        }
    }
}
