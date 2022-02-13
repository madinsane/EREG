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
    /// Describes a tooltip
    /// </summary>
    public class Tooltip : MonoBehaviour
    {
        public Text tooltip;

        /// <summary>
        /// Adds a tooltip
        /// </summary>
        /// <param name="text">Tooltip text</param>
        public void AddTooltip(string text)
        {
            text = text.Replace("\\n", "\n");
            text = text.Replace("==", "\n");
            tooltip.text = text;
        }

        /// <summary>
        /// Clears a tooltip
        /// </summary>
        public void ClearTooltip()
        {
            tooltip.text = "";
        }
    }
}
