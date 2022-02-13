using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Controls display of hits
    /// </summary>
    public class HitDisplay : MonoBehaviour
    {
        public TextMeshProUGUI damage;
        public TextMeshProUGUI weak;
        public TextMeshProUGUI technical;
        public TextMeshProUGUI crit;
        public Image healthBar;

        /// <summary>
        /// Controls health bar display
        /// </summary>
        /// <param name="unit">Unit to change</param>
        public void UpdateFill(Unit unit)
        {
            if (healthBar == null)
            {
                return;
            }
            healthBar.gameObject.SetActive(true);
            float value = (float)unit.CurrentHealth / unit.Stats.MaxHealth;
            healthBar.fillAmount = value;
        }

        /// <summary>
        /// Hides all effect text
        /// </summary>
        public void HideAll()
        {
            damage.gameObject.SetActive(false);
            weak.gameObject.SetActive(false);
            technical.gameObject.SetActive(false);
            crit.gameObject.SetActive(false);
        }

        /// <summary>
        /// Hides health bars
        /// </summary>
        public void HideHealth()
        {
            healthBar.gameObject.SetActive(false);
        }
    }
}
