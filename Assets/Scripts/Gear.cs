﻿using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    /// <summary>
    /// Defines an item
    /// </summary>
    public class Gear
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameStr { get; set; }
        public Constants.Slot Slot { get; set; }
        public string SpriteName { get; set; }
        public int RLvl { get; set; }
        public int Affixes { get; set; }
        [Ignore]
        public List<Modifier> mods;

        public Gear Copy()
        {
            return (Gear)MemberwiseClone();
        }
    }
}
