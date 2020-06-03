using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Modifier
    {
        public int Id { get; set; }
        public string Stat { get; set; }
        public string Description { get; set; }
        public float Value { get; set; }
        [Ignore]
        public float RealValue { get; set; }

        public Modifier Copy()
        {
            return (Modifier)MemberwiseClone();
        }
    }
}
