using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Controls the menu screen
    /// </summary>
    public class Menu : MonoBehaviour
    {
        public GameObject quitPanel;
        public Text quitText;

        /// <summary>
        /// Starts a new game
        /// </summary>
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

        /// <summary>
        /// Quits the game
        /// </summary>
        public void QuitGame()
        {
            //quitText.text = Application.streamingAssetsPath.ToString();
            Application.Quit(0);
        }

        /// <summary>
        /// Opens quit confirmation
        /// </summary>
        public void OpenQuitPanel()
        {
            quitPanel.SetActive(true);
            
        }

        /// <summary>
        /// Closes quit confirmation
        /// </summary>
        public void CloseQuitPanel()
        {
            quitPanel.SetActive(false);
        }

        /// <summary>
        /// Ends game
        /// </summary>
        public void EndGame()
        {
            SceneManager.LoadScene(0);
        }
    }
}
