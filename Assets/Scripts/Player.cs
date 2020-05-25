using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : Unit
    {
        public Player(UnitStats stats, List<SkillStats> skills) : base(stats, skills)
        {
            
        }
    }
}
