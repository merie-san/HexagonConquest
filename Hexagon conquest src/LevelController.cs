using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField] Button[] levelButtons;
    [SerializeField] Button backButton;
    public static int completedLevels = 0;

    private void Start()
    {
        SetButtons();
        int previousSession = PlayerPrefs.GetInt("completedLevels", 0);
        completedLevels = previousSession;
    }

    public void OnClick1()
    {
        SceneManager.LoadScene(2);
    }

    public void OnClick2()
    {
        SceneManager.LoadScene(3);
    }

    public void OnClick3()
    {
        SceneManager.LoadScene(4);
    }
    public void OnClick4()
    {
        SceneManager.LoadScene(5);
    }

    public void OnClick5()
    {
        SceneManager.LoadScene(6);
    }

    public void OnClick6()
    {
        SceneManager.LoadScene(7);
    }
    public void OnClick7()
    {
        SceneManager.LoadScene(8);
    }

    public void OnClick8()
    {
        SceneManager.LoadScene(9);
    }

    public void OnClick9()
    {
        SceneManager.LoadScene(10);
    }
    public void OnClick10()
    {
        SceneManager.LoadScene(11);
    }

    public void OnClick11()
    {
        SceneManager.LoadScene(12);
    }

    public void OnClick12()
    {
        SceneManager.LoadScene(13);
    }
    public void OnClickBack()
    {
        SceneManager.LoadScene(0);
    }

    public void SetButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i <= completedLevels)
            {
                levelButtons[i].interactable = true;
                levelButtons[i].GetComponentInChildren<TMP_Text>().text = "LEVEL " + (i + 1);
            }
            else
            {
                levelButtons[i].interactable = false;
                levelButtons[i].GetComponentInChildren<TMP_Text>().text = "";
            }
        }
    }
}
