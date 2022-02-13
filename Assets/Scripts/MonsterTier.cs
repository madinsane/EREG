using CsvHelper.Configuration.Attributes;

namespace Assets.Scripts
{
    /// <summary>
    /// Defines a tier of monsters
    /// </summary>
    class MonsterTier
    {
        public int Tier { get; set; }
        public Constants.TierType Type1 { get; set; }
        public string MonsterName1 { get; set; }
        public int Weight1 { get; set; }
        public Constants.TierType Type2 { get; set; }
        public string MonsterName2 { get; set; }
        public int Weight2 { get; set; }
        public Constants.TierType Type3 { get; set; }
        public string MonsterName3 { get; set; }
        public int Weight3 { get; set; }
        public Constants.TierType Type4 { get; set; }
        public string MonsterName4 { get; set; }
        public int Weight4 { get; set; }
        public Constants.TierType Type5 { get; set; }
        public string MonsterName5 { get; set; }
        public int Weight5 { get; set; }
        //Compiled data
        [Ignore]
        public Constants.TierType[] Types { get; set; }
        [Ignore]
        public string[] MonsterNames { get; set; }
        [Ignore]
        public int[] Weights { get; set; }
        [Ignore]
        public int TotalWeight { get; set; }
    }
}
