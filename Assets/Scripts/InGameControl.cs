using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameControl : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Restart Game");
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        Debug.Log("To Main Menu");
    }

    public void OpenMenu()
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 0;
        Debug.Log("Open Menu");
    }

    public void CloseMenu()
    {
        _pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Debug.Log("Close Menu");
    }
}
