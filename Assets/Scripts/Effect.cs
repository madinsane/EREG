using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Effect
    {
        public Constants.EffectType Type { get; set; }
        public Constants.BuffTypes BuffType { get; set; }
        public Constants.StatusTypes StatusType { get; set; }
        public int Power { get; set; }
        public int Duration { get; set; }

        public Effect(Constants.EffectType type, Constants.BuffTypes buffType = Constants.BuffTypes.None,
            Constants.StatusTypes statusType = Constants.StatusTypes.None, int power = 0, int duration = 2)
        {
            Type = type;
            BuffType = buffType;
            StatusType = statusType;
            Power = power;
            Duration = duration;
        }
    }
}
