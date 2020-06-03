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
            ClearEffects();
            IsDown = false;
            ColorChange();
            ChangeUnit(stats, skills, nameStr);
        }

        protected void ColorChange()
        {
            if (GetStatus() != Constants.StatusTypes.None)
            {
                spriteRenderer.color = new Color(129f/255, 255f/255, 233f/255, 255f/255);
            } else if (IsDown)
            {
                spriteRenderer.color = new Color(255f/255, 237f/255, 112f/255, 255f/255);
            } else
            {
                spriteRenderer.color = Color.white;
            }
        }

        public override void TakeHit(Damage.DamagePacket hit)
        {
            if (hit.hit)
            {
                if (hit.damage > 0)
                {
                    ChangeHealth(-hit.damage);
                }
                if (hit.status != Constants.StatusTypes.None)
                {
                    AddStatus(hit.status, hit.statusPower);
                }
                if (hit.removeStatus)
                {
                    RemoveStatus();
                }
                if (!IsDown && (hit.isWeak || hit.isCrit))
                {
                    IsDown = true;
                }
                ColorChange();
            }
        }

        public override void Die()
        {
            spriteRenderer.enabled = false;
            unitManager.ActiveMonsters--;
            base.Die();
        }
    }
}
