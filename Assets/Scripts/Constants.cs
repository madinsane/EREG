﻿namespace Assets.Scripts
{
    public static class Constants
    {
        public const string DATA_PATH = "/Data/";
        public const string UNIT_PATH = "unit.txt";
        public const string SKILL_PATH = "skill.txt";
        public const string MONSTER_PATH = "monster.txt";

        public const float TECHNICAL_MULTI = 2f;
        public const float CRITICAL_STATUS_MULTI = 2f;
        public const int MAX_ENEMIES = 5;
        public const int ENEMY_SKILL_TYPE_MAX = 5;
        public const float COST_SKILL_VARIANCE = 0.25f;
        public const int FORCED_SKILLS = 1;
        public const float DOWNGRADE_PENALTY = 0.1f;
        public const float UPGRADE_BONUS = 0.1f;

        public enum CostTypes
        {
            Attack, Spell
        }
        public enum SkillTypes
        {
            None, Almighty, Physical, Projectile, Electric, Cold, Fire,
            Wind, Arcane, Psychic, Light, Dark, Heal, Buff, Passive,
            Status, Blast, Hidden
        }
        public enum TierType
        {
            Empty, Monster, Upgrade, Downgrade
        }
        public enum TargetTypes
        {
            Single, All
        }
        public enum DamageTypes
        {
            Almighty, Physical, Projectile, Electric, Cold, Fire,
            Wind, Arcane, Psychic, Light, Dark
        }
        public enum StatusTypes
        {
            None, Shock, Freeze, Burn, Curse, Blast,
            Sleep, Forget, Berserk, Confuse, Brainwash, Fear
        }
        public enum BuffTypes
        {
            None, Damage, Defense, Evasion, Speed
        }
    }
}
