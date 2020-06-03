using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Menu : MonoBehaviour
    {
        public GameObject quitPanel;

        public void NewGame()
        {
            if (ExperimentControl.set)
            {
                int activateExperiment = Damage.RandomInt(0, 1);
                if (activateExperiment == 0)
                {
                    ExperimentControl.active = true;
                } else
                {
                    ExperimentControl.active = false;
                }
                ExperimentControl.set = true;
            } else
            {
                if (ExperimentControl.active)
                {
                    ExperimentControl.active = false;
                } else
                {
                    ExperimentControl.active = true;
                }
            }
            SceneManager.LoadScene(1);
        }

        public void QuitGame()
        {
            Application.Quit(0);
        }

        public void OpenQuitPanel()
        {
            quitPanel.SetActive(true);
            
        }

        public void CloseQuitPanel()
        {
            quitPanel.SetActive(false);
        }

        public void EndGame()
        {
            SceneManager.LoadScene(0);
        }
    }
}
