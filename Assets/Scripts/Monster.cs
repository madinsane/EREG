using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Monster : Unit
    {
        public SpriteRenderer spriteRenderer;
        public Monster(UnitStats stats, List<SkillStats> skills) : base(stats, skills)
        {
            
        }

        public void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.enabled = true;
        }

        public void ChangeMonster(UnitStats stats, List<SkillStats> skills, Sprite sprite)
        {
            SetSprite(sprite);
            ChangeUnit(stats, skills);
        }

        public new void Die()
        {
            spriteRenderer.enabled = false;
            unitManager.ActiveMonsters--;
            base.Die();
        }
    }
}
