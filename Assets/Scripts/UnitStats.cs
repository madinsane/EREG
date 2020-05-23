using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class UnitStats
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxHealth { get; set; }
        public int MaxMana { get; set; }
        public int AttackPower { get; set; }
        public int MagicPower { get; set; }
        public int AttackDefense { get; set; }
        public int MagicDefense { get; set; }
        public int Accuracy { get; set; }
        public int Evasion { get; set; }
        public int CritChance { get; set; }
        public int CritMulti { get; set; }
        public int IncCritChance { get; set; }
        public int TypeStatusChance { get; set; }
        public int MentalStatusChance { get; set; }
        public int IncTypeStatus { get; set; }
        public int IncMentalStatus { get; set; }
        public int ResistPhysical { get; set; }
        public int ResistProjectile { get; set; }
        public int ResistElectric { get; set; }
        public int ResistCold { get; set; }
        public int ResistFire { get; set; }
        public int ResistWind { get; set; }
        public int ResistArcane { get; set; }
        public int ResistPsychic { get; set; }
        public int ResistLight { get; set; }
        public int ResistDark { get; set; }
        public Constants.StatusTypes Status { get; set; }
    }
}
