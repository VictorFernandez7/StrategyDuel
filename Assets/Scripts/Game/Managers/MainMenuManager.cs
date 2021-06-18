using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Game.Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        public void LoadPlay()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}