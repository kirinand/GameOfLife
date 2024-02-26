using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _sideSelectionMenu;

    public void PlayGame(int ai) // 0 for white, 1 for black, -1 if no AI
    {   
        PlayerPrefs.SetInt("AI", ai);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OpenSideSelectionMenu()
    {
        _sideSelectionMenu.SetActive(true);
    }

    public void CloseSideSelectionMenu() 
    {
        _sideSelectionMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
