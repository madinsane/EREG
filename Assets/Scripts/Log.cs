using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Log : MonoBehaviour
    {
        private List<string> log;
        private List<string> fullLog;
        private string displayText;
        private string logName;
        public int maxLines = 50;
        public Text textBox;
        private bool doneHeader = false;
        // Start is called before the first frame update
        void Awake()
        {
            log = new List<string>();
            fullLog = new List<string>();
            displayText = "";
            textBox.text = displayText;
            logName = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
            File.Create(Application.dataPath + Constants.LOG_PATH + logName + ".txt");
        }

        internal void Add(string line)
        {
            if (log.Count >= maxLines)
            {
                log.RemoveAt(0);
            }
            log.Add(line);
            fullLog.Add(line);
            displayText = "";
            foreach (string entry in log)
            {
                displayText += entry + "\n";
            }
            textBox.text = displayText;
            using (StreamWriter w = File.AppendText(Application.dataPath + Constants.LOG_PATH + logName + ".txt"))
            {
                if (!doneHeader)
                {
                    w.WriteLine("ExperimentControl active: " + ExperimentControl.active.ToString());
                    w.WriteLine("ExperimentControl set: " + ExperimentControl.set.ToString());
                    doneHeader = true;
                }
                w.WriteLine(line);
            }
        }
    }
}