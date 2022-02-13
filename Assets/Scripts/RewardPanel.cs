using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Describes a reward panel
    /// </summary>
    public class RewardPanel : MonoBehaviour
    {
        public RewardManager rewards;
        public Constants.RewardTypes rewardType;
        public Tooltip tooltip;
        public Image image;
        private SkillStats skill;
        private Gear gear;
        private ItemStats item;

        /// <summary>
        /// Changes the rewards and sprites
        /// </summary>
        /// <param name="reward">Reward object</param>
        /// <param name="sprite">Reward sprite</param>
        public void ChangeReward(object reward, Sprite sprite)
        {
            switch (rewardType)
            {
                case Constants.RewardTypes.Skill:
                    skill = (SkillStats)reward;
                    break;
                case Constants.RewardTypes.Gear:
                    gear = (Gear)reward;
                    break;
                case Constants.RewardTypes.Item:
                    item = (ItemStats)reward;
                    break;
            }
            image.sprite = sprite;
        }

        /// <summary>
        /// Chooses a reward
        /// </summary>
        public void ChooseReward()
        {
            switch (rewardType)
            {
                case Constants.RewardTypes.Skill:
                    rewards.PickedReward(rewardType, skill);
                    break;
                case Constants.RewardTypes.Gear:
                    rewards.PickedReward(rewardType, gear);
                    break;
                case Constants.RewardTypes.Item:
                    rewards.PickedReward(rewardType, item);
                    break;
            }
        }

        /// <summary>
        /// Displays reward tooltip
        /// </summary>
        public void DisplayTooltip()
        {
            StringBuilder sb = new StringBuilder();
            switch (rewardType)
            {
                case Constants.RewardTypes.Skill:
                    sb.Append(skill.NameStr + "\n" + "Cost: " + skill.Cost + (skill.CostType == Constants.CostTypes.Attack ? "HP" : "MP") + "\n" + skill.Description);
                    tooltip.AddTooltip(sb.ToString());
                    break;
                case Constants.RewardTypes.Gear:
                    if (gear == null)
                    {
                        return;
                    }
                    if (gear.mods != null)
                    {
                        foreach (Modifier mod in gear.mods)
                        {
                            sb.Append(mod.RealValue.ToString() + mod.Description + "\n");
                        }
                        tooltip.AddTooltip(gear.NameStr + "\n" + sb.ToString());
                    }
                    else
                    {
                        tooltip.AddTooltip(gear.NameStr);
                    }
                    break;
                case Constants.RewardTypes.Item:
                    sb.Append(item.NameStr + "\n" + item.Description);
                    tooltip.AddTooltip(sb.ToString());
                    break;
            }
        }

    }
}
