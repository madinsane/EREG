namespace Assets.Scripts
{
    public class SkillStats
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public Constants.CostTypes CostType { get; set; }
        public Constants.TargetTypes TargetType { get; set; }
        public Constants.DamageTypes DamageType { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }
        public int CritChance { get; set; }
        public int CritMulti { get; set; }
        public Constants.StatusTypes StatusType { get; set; }
        public int StatusChance { get; set; }
        public int StatusPower { get; set; }
    }
}
