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
    /// Defines a gear panel
    /// </summary>
    public class GearPanel : MonoBehaviour
    {
        public Tooltip tooltip;
        public Image gearImage;
        private Gear gear;
        
        /// <summary>
        /// Shows the tooltip for a gear piece
        /// </summary>
        public void DisplayTooltip()
        {
            if (gear == null)
            {
                return;
            }
            if (gear.mods != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Modifier mod in gear.mods)
                {
                    sb.Append(mod.RealValue.ToString() + mod.Description + "\n");
                }
                tooltip.AddTooltip(gear.NameStr + "\n" + sb.ToString());
            } else
            {
                tooltip.AddTooltip(gear.NameStr);
            }
        }

        /// <summary>
        /// Changes the gear displayed
        /// </summary>
        /// <param name="gear">New gear</param>
        /// <param name="sprite">New sprite</param>
        public void ChangeGear(Gear gear, Sprite sprite)
        {
            this.gear = gear;
            gearImage.sprite = sprite;
        }
    }
}
