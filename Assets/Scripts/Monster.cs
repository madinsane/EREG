using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Monster : Unit
    {
        public SpriteRenderer spriteRenderer;
        public Monster(int id) : base(id)
        {
            
        }

        public void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
    }
}
