using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomePageController : MonoBehaviour
{
    public void OnClickLevels()
    {
        SceneManager.LoadScene(1);
    }

    public void OnClickQuit()
    {
        Application.Quit();
        PlayerPrefs.SetInt("completedLevels", LevelController.completedLevels);
        PlayerPrefs.Save();
    }
}
