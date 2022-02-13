using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    /// <summary>
    /// Defines an effect
    /// </summary>
    public class Effect
    {
        public Constants.EffectType Type { get; set; }
        public Constants.BuffTypes BuffType { get; set; }
        public Constants.StatusTypes StatusType { get; set; }
        public float Power { get; set; }
        public int Duration { get; set; }

        /// <summary>
        /// Initialises an effect
        /// </summary>
        /// <param name="type">Type of effect</param>
        /// <param name="buffType">Type of buff</param>
        /// <param name="statusType">Type of status</param>
        /// <param name="power">Power of effect</param>
        /// <param name="duration">Duration of effect</param>
        public Effect(Constants.EffectType type, Constants.BuffTypes buffType = Constants.BuffTypes.None,
            Constants.StatusTypes statusType = Constants.StatusTypes.None, float power = 0, int duration = 2)
        {
            Type = type;
            BuffType = buffType;
            StatusType = statusType;
            Power = power;
            Duration = duration;
        }
    }
}
