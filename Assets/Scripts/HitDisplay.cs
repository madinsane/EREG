using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class HitDisplay : MonoBehaviour
    {
        public TextMeshProUGUI damage;
        public TextMeshProUGUI weak;
        public TextMeshProUGUI technical;
        public TextMeshProUGUI crit;

        public void HideAll()
        {
            damage.gameObject.SetActive(false);
            weak.gameObject.SetActive(false);
            technical.gameObject.SetActive(false);
            crit.gameObject.SetActive(false);
        }
    }
}
