using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Log : MonoBehaviour
    {
        private List<string> log;
        private string displayText;
        public int maxLines = 50;
        public Text textBox;
        // Start is called before the first frame update
        void Awake()
        {
            log = new List<string>();
            displayText = "";
            textBox.text = displayText;
        }

        void Add(string line)
        {
            if (log.Count >= maxLines)
            {
                log.RemoveAt(0);
            }
            log.Add(line);
            displayText = "";
            foreach (string entry in log)
            {
                displayText += entry;
            }
            textBox.text = displayText;
        }
    }
}