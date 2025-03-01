using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuPopup : MonoBehaviour
{
    [SerializeField] private Button returnButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button backToLevelOneButton;

    public void Open()
    {
        gameObject.SetActive(true);
        Messenger<bool>.Broadcast(GameEvents.GAME_STATUS_CHANGED, true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Messenger<bool>.Broadcast(GameEvents.GAME_STATUS_CHANGED, false);
    }

    public void SetButtons()
    {
        returnButton.interactable = true;
        restartButton.interactable = true;
        nextLevelButton.interactable = false;
        homeButton.interactable = true;
        backToLevelOneButton.interactable = false;
        int currentLevel = SceneManager.GetActiveScene().buildIndex - 1;
        if (LevelController.completedLevels >= currentLevel && currentLevel != 12)
        {
            nextLevelButton.interactable = true;
        }
        else if (currentLevel == 12)
        {
            backToLevelOneButton.interactable = true;
        }
    }

    public void SetButtonsWinLev()
    {
        returnButton.interactable = false;
        restartButton.interactable = true;
        nextLevelButton.interactable = true;
        homeButton.interactable = true;
        backToLevelOneButton.interactable = false;
        if (IsLastLevel())
        {
            backToLevelOneButton.interactable = true;
            nextLevelButton.interactable = false;
            backToLevelOneButton.GetComponentInChildren<TMP_Text>().text = "Congratulations, you cleared every level on this game!!! you can now go to the first level with this button on the right.";
        }
    }

    public void SetButtonsLoseLev()
    {
        returnButton.interactable = false;
        restartButton.interactable = true;
        nextLevelButton.interactable = false;
        homeButton.interactable = true;
        backToLevelOneButton.interactable = false;
    }

    public void OnReturnButtonClicked()
    {
        Close();
    }

    public void OnRestartButtonClicked()
    {
        Scene currentLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentLevel.buildIndex);
    }

    public bool IsLastLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex - 1;
        return currentLevel >= 12;
    }

    public void OnNextLevelButtonClicked()
    {
        Scene currentLevel = SceneManager.GetActiveScene();
        int nextLevelBuildIndex = currentLevel.buildIndex + 1;
        if (!IsLastLevel())
        {
            SceneManager.LoadScene(nextLevelBuildIndex);
        }
        else
        {
            Debug.Log("next level doesn't exist");
        }
    }

    private bool IsFirstLevel()
    {
        Scene currentLevel = SceneManager.GetActiveScene();
        return currentLevel.buildIndex == 1;
    }

    public void OnHomeButtonClicked()
    {
        Scene currentLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(0);
    }

    public void OnBackToFirstLevelButtonClicked()
    {
        Scene currentLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(1);
    }
}
