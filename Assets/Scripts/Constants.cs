namespace Assets.Scripts
{
    public static class Constants
    {
        public const string DATA_PATH = "/data";
        public const string UNIT_PATH = "/unit.txt";

        public const float TECHNICAL_MULTI = 2f;

        public enum SkillTypes
        {
            Attack, Spell
        }
        public enum TargetTypes
        {
            Single, All
        }
        public enum DamageTypes
        {
            Almighty, Physical, Projectile, Electric, Cold, Fire, Wind, Arcane, Psychic, Light, Dark
        }
        public enum StatusTypes
        {
            None, Shock, Freeze, Burn, Curse, Blast, Sleep, Forget, Berserk, Confuse, Brainwash, Fear
        }
    }
}
