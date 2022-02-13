using CsvHelper.Configuration.Attributes;

namespace Assets.Scripts
{
    /// <summary>
    /// Describes stats of a unit
    /// </summary>
    public class UnitStats
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float MaxHealth { get; set; }
        public float MaxMana { get; set; }
        public float AttackPower { get; set; }
        public float MagicPower { get; set; }
        public int Speed { get; set; }
        public float AttackDefense { get; set; }
        public float MagicDefense { get; set; }
        public float Accuracy { get; set; }
        public float Evasion { get; set; }
        public float CritChance { get; set; }
        public float CritMulti { get; set; }
        public float IncCritChance { get; set; }
        public float TypeStatusChance { get; set; }
        public float MentalStatusChance { get; set; }
        public float StatusPower { get; set; }
        public float IncTypeStatus { get; set; }
        public float IncMentalStatus { get; set; }
        public float ResistPhysical { get; set; }
        public float ResistProjectile { get; set; }
        public float ResistElectric { get; set; }
        public float ResistCold { get; set; }
        public float ResistFire { get; set; }
        public float ResistWind { get; set; }
        public float ResistArcane { get; set; }
        public float ResistPsychic { get; set; }
        public float ResistLight { get; set; }
        public float ResistDark { get; set; }
        [Ignore]
        public Constants.StatusTypes Status { get; set; }

        public UnitStats Copy()
        {
            return (UnitStats)MemberwiseClone();
        }

    }
}
