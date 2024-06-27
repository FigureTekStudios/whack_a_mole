using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int preGameCountdownTime = 3; // pre-game countdown time in seconds
    public int initialGameTime = 60; // this should be determined by song length
    public TextMeshProUGUI preGameCountdownText;
    public TextMeshProUGUI countdownText; 
    public TextMeshProUGUI scoreText; 

    private int score;
    private float gameTimer;
    private float preGameTimer;
    private bool gameStarted;
    private bool gameEnded;

    private GameBoard board; // this should probably another singleton

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        board = GameObject.Find("GameBoard").GetComponent<GameBoard>();
    }

    private void Start()
    {
        InitializeGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            InitializeGame();
        }

        if (!gameStarted)
        {
            UpdatePreGameCountdown();
        }
        else if (!gameEnded)
        {
            UpdateGameCountdown();
        }
    }


    private void UpdatePreGameCountdown()
    {
        preGameTimer -= Time.deltaTime;
        if (preGameTimer <= 0)
        {
            preGameTimer = 0;
            StartGame();
        }
        UpdatePreGameCountdownText(preGameTimer);
    }


    private void InitializeGame()
    {
        score = 0;
        gameTimer = initialGameTime;
        preGameTimer = preGameCountdownTime;
        gameStarted = false;
        gameEnded = false;
        StartCoroutine(board.GenerateGameBoard());

        UpdateScoreText();

        UpdatePreGameCountdownText(preGameTimer);
    }

    private void StartGame()
    {
        gameStarted = true;
        preGameCountdownText.gameObject.SetActive(false); // Hide the pre-game countdown text
        UpdateGameCountdownText(gameTimer);
    }

    private void EndGame()
    {
        gameEnded = true;
        Debug.Log("Game Over! Final Score: " + score);
        // Implement additional game over logic here (e.g., show game over screen)
    }


    public void AddScore(int amount)
    {
        if (!gameEnded)
        {
            score += amount;
            UpdateScoreText();
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void UpdatePreGameCountdownText(float timer)
    {
        if (preGameCountdownText != null)
        {
            preGameCountdownText.text = "Game starts in: " + Mathf.CeilToInt(timer);
        }
    }

    private void UpdateGameCountdown()
    {
        gameTimer -= Time.deltaTime;
        if (gameTimer <= 0)
        {
            gameTimer = 0;
            EndGame();
        }
        UpdateGameCountdownText(gameTimer);
    }

    private void UpdateGameCountdownText(float timer)
    {
        if (countdownText != null)
        {
            countdownText.text = "Time: " + Mathf.CeilToInt(timer);
        }
    }
}
