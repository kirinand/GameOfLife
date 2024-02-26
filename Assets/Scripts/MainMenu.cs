using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _sideSelectionMenu;
    [SerializeField] private GameObject _rulePanel;

    public void PlayGame(int ai) // 0 for white, 1 for black, -1 if no AI
    {   
        PlayerPrefs.SetInt("AI", ai);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ToggleSideSelectionMenu(bool state)
    {
        _sideSelectionMenu.SetActive(state);
    }

    public void ToggleRulePanel(bool state)
    {
        _rulePanel.SetActive(state);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
