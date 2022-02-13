using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Defines a monster
    /// </summary>
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

        /// <summary>
        /// Sets monster sprite
        /// </summary>
        /// <param name="sprite">Sprite to use</param>
        public void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.enabled = true;
        }

        /// <summary>
        /// Changes the current monster
        /// </summary>
        /// <param name="stats">Monster stats</param>
        /// <param name="skills">Monster skills</param>
        /// <param name="sprite">Sprite to use</param>
        /// <param name="nameStr">Monster name</param>
        public void ChangeMonster(UnitStats stats, List<SkillStats> skills, Sprite sprite, string nameStr)
        {
            IsPlayer = false;
            SetSprite(sprite);
            ClearEffects();
            IsDown = false;
            ColorChange();
            ChangeUnit(stats, skills, nameStr);
        }

        /// <summary>
        /// Changes sprite color
        /// </summary>
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

        /// <summary>
        /// Adds a status
        /// </summary>
        /// <param name="type">Type of status</param>
        /// <param name="statusPower">Power of status</param>
        /// <param name="duration">Status duration in turns (default 2)</param>
        public override void AddStatus(Constants.StatusTypes type, int statusPower, int duration = 2)
        {
            if (Effects == null)
            {
                Effects = new List<Effect>();
            }
            RemoveStatus();
            Effects.Add(new Effect(Constants.EffectType.Status, Constants.BuffTypes.None, type, statusPower, duration));
            if (type == Constants.StatusTypes.Berserk || type == Constants.StatusTypes.Confuse)
            {
                if (type == Constants.StatusTypes.Berserk)
                {
                    Stats.AttackDefense /= Constants.BERSERK_MODIFIER;
                    Stats.AttackPower /= Constants.BERSERK_MODIFIER;
                    Stats.MagicDefense /= Constants.BERSERK_MODIFIER;
                    unitManager.log.Add(NameStr + " Berserks");
                }
                else
                {
                    Stats.Accuracy /= Constants.CONFUSE_MODIFIER;
                    Stats.Evasion /= Constants.CONFUSE_MODIFIER;
                    unitManager.log.Add(NameStr + " is Confused");
                }
            }
            ColorChange();
        }

        /// <summary>
        /// Updates the sprite color
        /// </summary>
        public override void UpdateColor()
        {
            ColorChange();
        }

        /// <summary>
        /// Applies damage packet to monster
        /// </summary>
        /// <param name="hit">Packet to use</param>
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
                    AddStatus(hit.status, hit.statusPower, hit.statusDuration);
                    Stats.Status = hit.status;
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

        /// <summary>
        /// Kills the monster
        /// </summary>
        public override void Die()
        {
            spriteRenderer.enabled = false;
            unitManager.ActiveMonsters--;
            base.Die();
        }
    }
}
