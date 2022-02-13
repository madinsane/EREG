namespace Assets.Scripts
{

    /// <summary>
    /// Defines the global constants
    /// </summary>
    public static class Constants
    {
        public const string DATA_PATH = "/Data/";
        public const string LOG_PATH = "/Logs/";
        public const string UNIT_PATH = "unit.txt";
        public const string SKILL_PATH = "skill.txt";
        public const string MONSTER_PATH = "monster.txt";
        public const string MONSTERTIER_PATH = "monsterTier.txt";
        public const string HELP_PATH = "help.txt";
        public const string ITEM_PATH = "item.txt";
        public const string MODIFIER_PATH = "modifier.txt";
        public const string GEAR_PATH = "gear.txt";

        public const float TECHNICAL_MULTI = 2f;
        public const float CRITICAL_STATUS_MULTI = 2f;
        public const int MAX_ENEMIES = 5;
        public const int ENEMY_SKILL_TYPE_MAX = 5;
        public const int FORCED_SKILLS = 1;
        public const float DOWNGRADE_PENALTY = 0.1f;
        public const float UPGRADE_BONUS = 0.1f;
        public const float LEVELS_PER_TIER = 2f;
        public const int MAX_TIERS = 24;
        public const int MONSTER_SKILL_RLVL_PENALTY = 5;
        public const int SKILL_RLVL_LENIANCE = 5;
        public const int START_POTIONS = 3;
        public const int START_ELIXIR = 2;
        public const int ITEM_TYPES = 3;
        public const int MAX_SKILLS = 16;
        public const int BACKGROUND_CHANGE_LVLS = 10;
        public const int AFFIX_BASE_VALUE = 5;
        public const int FEAR_PLAYER_COST = 100;
        public const float CURSE_COST = 5f;
        public const float BERSERK_MODIFIER = 2f;
        public const float CONFUSE_MODIFIER = 3f;
        public const float GUARD_MODIFIER = 2f;
        public const float BUFF_MULTIPLIER = 1.5f;
        public const float CHEAT_THRESHOLD = 0.25f;
        public const int SPEED_REDUCTION = 25;

        public const string DISABLED_GUI_BACK = "disabled-base";
        public const string ENABLED_GUI_BACK = "enabled-base";
        public const string BACKGROUND_BASE = "battleback";

        public enum ActionBoxTextTypes
        {
            Stats, Help
        }
        public enum CostTypes
        {
            Attack, Spell
        }
        public enum SkillTypes
        {
            None, Almighty, Physical, Projectile, Electric, Cold, Fire,
            Wind, Arcane, Psychic, Light, Dark, Heal, Buff, Passive,
            Status, Blast, Break, Hidden
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
            None, Damage, Defense, Evasion, Speed, Guard
        }
        public enum ItemTypes
        {
            Health, Mana, Break
        }
        public enum EffectType
        {
            None, Status, Buff
        }
        public enum Slot
        {
            Helm, Chest, Gloves, Boots, Weapon,
            Shield, Amulet, Ring
        }
        public enum RewardTypes
        {
            Skill, Gear, Item
        }
    }
}
