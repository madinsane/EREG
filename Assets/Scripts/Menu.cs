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
