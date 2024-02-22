using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUIControl : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private TextMeshProUGUI _whiteScoreText;
    [SerializeField] private TextMeshProUGUI _blackScoreText;
    private static InGameUIControl _instance;

    public static InGameUIControl Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("InGameControl is null");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void OpenMenu()
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void CloseMenu()
    {
        _pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void UpdateScoreUI(int whiteScore, int blackScore)
    {
        _whiteScoreText.text = whiteScore.ToString();
        _blackScoreText.text = blackScore.ToString();
    }   
}
