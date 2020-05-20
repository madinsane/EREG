namespace Assets.Scripts
{
    public static class Constants
    {
        public const string DATA_PATH = "/data";
        public const string UNIT_PATH = "/unit.txt";

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
            Physical, Projectile, Electric, Cold, Fire
        }
    }
}
