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
    public class HitDisplay : MonoBehaviour
    {
        public TextMeshProUGUI damage;
        public TextMeshProUGUI weak;
        public TextMeshProUGUI technical;
        public TextMeshProUGUI crit;
        public Image healthBar;

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

        public void HideAll()
        {
            damage.gameObject.SetActive(false);
            weak.gameObject.SetActive(false);
            technical.gameObject.SetActive(false);
            crit.gameObject.SetActive(false);
        }

        public void HideHealth()
        {
            healthBar.gameObject.SetActive(false);
        }
    }
}
