using CsvHelper.Configuration.Attributes;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Defines monster data
    /// </summary>
    public class MonsterData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SpriteName { get; set; }
        public string NameStr { get; set; }
        public int UnitId { get; set; }
        public int Value { get; set; }
        public Constants.SkillTypes SkillType1 { get; set; }
        public Constants.SkillTypes SkillType2 { get; set; }
        public Constants.SkillTypes SkillType3 { get; set; }
        public Constants.SkillTypes SkillType4 { get; set; }
        public Constants.SkillTypes SkillType5 { get; set; }
        [Ignore]
        public Constants.SkillTypes[] SkillTypeFull { get; set; }
        [Ignore]
        public UnitStats Unit { get; set; }
        [Ignore]
        public Sprite Sprite { get; set; }
        [Ignore]
        public float StatMulti { get; set; }
        [Ignore]
        public bool[] CheckedType { get; set; } = new bool[(int)Constants.DamageTypes.Dark];

        public MonsterData()
        {

        }
    }
}
