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

        }
        internal static int RandomInt(int min, int max)
        {
            if (random == null)
            {
                random = new System.Random();
            }
            return random.Next(min, max+1);
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
                case Constants.DamageTypes.Electric:
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
            //Check Crit
            bool isCrit = false;
            int critChance = skill.CritChance * (attacker.CritChance / 100);
            critChance *= defender.IncCritChance / 100;
            int critRoll = RandomInt(1, 100);
            if (critChance >= critRoll)
            {
                isCrit = true;
                int critMulti = attacker.CritMulti * (skill.CritMulti / 100);
                power *= critMulti / 100;
            }
            return new DamagePacket();
        }
    }
}
