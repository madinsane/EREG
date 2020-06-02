using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Tooltip : MonoBehaviour
    {
        public Text tooltip;

        public void AddTooltip(string text)
        {
            text = text.Replace("\\n", "\n");
            text = text.Replace("==", "\n");
            tooltip.text = text;
        }

        public void ClearTooltip()
        {
            tooltip.text = "";
        }
    }
}
