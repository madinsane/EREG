
using CsvHelper.Configuration.Attributes;

namespace Assets.Scripts
{
    public class ItemStats
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameStr { get; set; }
        public string Description { get; set; }
        public string SpriteName { get; set; }
        public Constants.ItemTypes Type { get; set; }
        public int Value { get; set; }
        [Ignore]
        public int Quantity { get; set; }
    }
}
