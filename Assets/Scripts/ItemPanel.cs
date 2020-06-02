using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ItemPanel : MonoBehaviour
    {
        public Image itemBack;
        public Image item;
        public Text cost;
        public Image costBack;
        public Sprite skillBackImg;
        public Sprite skillBackImgDisabled;
        public Sprite itemBackImg;
        public Player player;

        public Color itemColor;
        public Color hpColor;
        public Color mpColor;
        public Color disabled;

        private int itemId = 0;
        private int skillCost = 0;
        private Constants.CostTypes costType;
        private bool isItem = false;

        public void ChangeSkill(int skillId, Sprite skill, int cost, Constants.CostTypes costType)
        {
            isItem = false;
            itemBack.sprite = skillBackImg;
            item.sprite = skill;
            string costSuffix;
            Color color;
            switch (costType)
            {
                case Constants.CostTypes.Attack:
                    costSuffix = "HP";
                    color = hpColor;
                    break;
                default:
                    costSuffix = "MP";
                    color = mpColor;
                    break;
            }
            this.cost.text = cost.ToString() + "\n" + costSuffix;
            this.cost.color = color;
            itemId = skillId;
            skillCost = cost;
            this.costType = costType;
            UpdateDisplay();
        }

        public void ChangeItem(int itemId, Sprite item)
        {
            isItem = true;
            this.itemId = itemId;
            itemBack.sprite = itemBackImg;
            this.item.sprite = item;
            cost.text = player.GetItem(itemId).Quantity.ToString();
            cost.color = itemColor;
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            if (isItem)
            {
                itemBack.color = Color.white;
                cost.text = player.GetItem(itemId).Quantity.ToString();
            } else
            {
                if (player.CheckCost(skillCost, costType))
                {
                    itemBack.color = Color.white;
                    itemBack.sprite = skillBackImg;
                } else
                {
                    itemBack.color = disabled;
                    itemBack.sprite = skillBackImgDisabled;
                }
            }
        }

        public void Activate()
        {
            if (isItem)
            {
                if (player.unitManager.Turn == UnitManager.Turns.Player)
                {
                    //Use item
                    player.UseItem(itemId);
                }
            } else
            {
                if (player.unitManager.Turn == UnitManager.Turns.Player && player.CheckCost(skillCost, costType))
                {
                    SkillStats skill = player.unitManager.GetSkill(itemId);
                    if (skill.SkillType == Constants.SkillTypes.Break)
                    {
                        if (player.GetStatus() != skill.StatusType)
                        {
                            return;
                        }
                    }
                    //Use skill
                    player.unitManager.PrepareSkill(skill);
                }
            }
            UpdateDisplay();
        }
    }
}
