using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ActionBox: MonoBehaviour
    {
        public Text text;
        private string helpText;


        private void Awake()
        {
            helpText = "";
        }

        public void DisplayHelp()
        {
            if (helpText == "" || helpText == null)
            {
                LoadHelp();
            }
            text.text = helpText;
        }

        private void LoadHelp()
        {
            helpText = File.ReadAllText(Application.dataPath + Constants.DATA_PATH + Constants.HELP_PATH);
        }
    }
}
