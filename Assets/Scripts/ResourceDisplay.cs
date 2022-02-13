using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Displays player resources
    /// </summary>
    public class ResourceDisplay : MonoBehaviour
    {
        public UnitManager unitManager;
        public Image resource;
        private float value = 1;
        void Awake()
        {
            value = GetResourcePercent();
            resource.fillAmount = value;
        }

        float GetResourcePercent()
        {
            return 1;
        }

        /// <summary>
        /// Sets globe fill amount
        /// </summary>
        /// <param name="newValue">Value to fill</param>
        public void SetGlobe(float newValue)
        {
            resource.fillAmount = newValue;
        }
    }
}
