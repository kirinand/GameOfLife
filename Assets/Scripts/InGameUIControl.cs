using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUIControl : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private TextMeshProUGUI _whiteScoreText;
    [SerializeField] private TextMeshProUGUI _blackScoreText;
    [SerializeField] private TextMeshProUGUI _totalTurnsText;
    [SerializeField] private TextMeshProUGUI _outcomeText;
    [SerializeField] private Image _winnerDisplay;
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

    public void GameOver(int winner, int totalTurn)
    {
        _gameOverScreen.SetActive(true);
        _totalTurnsText.text = totalTurn.ToString();

        if (winner == 0)
        {
            _winnerDisplay.sprite = Resources.Load<Sprite>("Sprites/WhiteStone");
            _outcomeText.text = "Winner";
        }

        else if (winner == 1) 
        {
            _winnerDisplay.sprite = Resources.Load<Sprite>("Sprites/BlackStone");
            _outcomeText.text = "Winner";
        }
        else
        {
            _winnerDisplay.sprite = null;
            _outcomeText.text = "Draw!";
        }

        Time.timeScale = 0;
    }
}
