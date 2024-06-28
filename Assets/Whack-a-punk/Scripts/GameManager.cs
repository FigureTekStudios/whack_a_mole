using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int preGameCountdownTime = 3; // pre-game countdown time in seconds
    public int initialGameTime = 60; // this should be determined by song length
    public int scoreToUnlockPowerUp = 100; // Score needed to unlock a power-up

    public TextMeshProUGUI preGameCountdownText;
    public TextMeshProUGUI countdownText; 
    public TextMeshProUGUI scoreText;

    public Image powerUpMeter; // UI Image to display the power-up meter
    public Image[] powerUpIcons; // UI Images to display power-up icons

    private int score;
    private float gameTimer;
    private float preGameTimer;
    private bool gameStarted;
    private bool gameEnded;
    private int powerUpCount;
    private float powerUpProgress;

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
            InitializeGame();

        if (!gameStarted)
            UpdatePreGameCountdown();

        else if (!gameEnded)
            UpdateGameCountdown();

    }

    private void InitializeGame()
    {
        StartCoroutine(board.GenerateGameBoard());

        score = 0;
        gameTimer = initialGameTime;
        preGameTimer = preGameCountdownTime;
        gameStarted = false;
        gameEnded = false;
        powerUpCount = 0;
        powerUpProgress = 0;

        UpdateTotalScoreText();
        UpdatePreGameCountdownText(preGameTimer);
        UpdatePowerUpMeter();
        UpdatePowerUpIcons();
    }

    private void StartGame()
    {
        gameStarted = true;
        preGameCountdownText.gameObject.SetActive(false); // Hide the pre-game countdown text
        countdownText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        UpdateGameCountdownText(gameTimer);
    }

    private void EndGame()
    {
        gameEnded = true;
        Debug.Log($"Game Over! Final Score: {score}");
        // Implement additional game over logic here (e.g., show game over screen)
    }

    public void AddScore(int amount)
    {
        if (gameStarted && !gameEnded)
        {
            score += amount;
            UpdateTotalScoreText();
            UpdatePowerUpProgress(amount);
        }
    }

    private void UpdateTotalScoreText()
    {
        if (scoreText != null)
            scoreText.text = $"{score}";
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

    private void UpdatePreGameCountdownText(float timer)
    {
        if (preGameCountdownText != null)
            preGameCountdownText.text = $"{Mathf.CeilToInt(timer)}";
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
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            int milliseconds = Mathf.FloorToInt((timer % 1) * 100);

            countdownText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }
    }

    private void UpdatePowerUpProgress(int amount)
    {
        if (powerUpCount < 3)
        {
            powerUpProgress += amount;

            if (powerUpProgress >= scoreToUnlockPowerUp)
            {
                powerUpProgress = 0;
                powerUpCount++;
                UpdatePowerUpIcons();
            }

            UpdatePowerUpMeter();
        }
    }

    private void UpdatePowerUpMeter()
    {
        if (powerUpMeter != null)
        {
            powerUpMeter.fillAmount = powerUpProgress / scoreToUnlockPowerUp;
        }
    }

    private void UpdatePowerUpIcons()
    {
        for (int i = 0; i < powerUpIcons.Length; i++)
        {
            if (powerUpIcons[i] != null)
            {
                powerUpIcons[i].enabled = i < powerUpCount;
            }
        }
    }
}
