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
            IsPlayer = false;
        }

        private void Awake()
        {
            IsPlayer = false;
        }

        public void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.enabled = true;
        }

        public void ChangeMonster(UnitStats stats, List<SkillStats> skills, Sprite sprite, string nameStr)
        {
            IsPlayer = false;
            SetSprite(sprite);
            ChangeUnit(stats, skills, nameStr);
        }

        public override void Die()
        {
            spriteRenderer.enabled = false;
            unitManager.ActiveMonsters--;
            base.Die();
        }
    }
}
