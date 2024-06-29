using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int preGameCountdownTime = 3; // pre-game countdown time in seconds
    public int initialGameTime = 60; // this should be determined by song length
    public int scoreToUnlockPowerUp = 100; // Score needed to unlock a power-up

    public TextMeshProUGUI preGameCountdownText;
    public TextMeshProUGUI countdownText; 
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI totalScoreText;

    public Image[] powerUpIcons; // UI Images to display power-up icons

    private GameBoard board; // this should probably be another singleton

    private int currentScore;
    private int totalScore;
    private float gameTimer;
    private float preGameTimer;
    private bool gameStarted;
    private bool gameEnded;
    private int powerUpCount;
    private float powerUpProgress;
    
    
    private Coroutine fadeCoroutine;
    private float currentScoreDisplayTime;
    private float currentScoreFadeDuration = 1;
    private float currentScoreVisibleDuration = 1f;


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
            UsePowerUp();

        if (!gameStarted)
            UpdatePreGameCountdown();
        else if (!gameEnded)
        {
            UpdateGameCountdown();
            HandleCurrentScoreTextFade();
        }
    }

    private void InitializeGame()
    {
        StartCoroutine(board.GenerateGameBoard());

        totalScore = 0;
        currentScore = 0;
        gameTimer = initialGameTime;
        preGameTimer = preGameCountdownTime;
        gameStarted = false;
        gameEnded = false;
        powerUpCount = 0;
        powerUpProgress = 0;

        UpdateTotalScoreText();
        SetCurrentScoreTextAlpha(0); // Start with current score text hiddenUpdateCurrentScoreText(0, 1);

        UpdatePreGameCountdownText(preGameTimer);
        UpdatePowerUpIcons();
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);
        gameStarted = true;
        preGameCountdownText.gameObject.SetActive(false); // Hide the pre-game countdown text
        countdownText.gameObject.SetActive(true);
        totalScoreText.gameObject.SetActive(true);
        UpdateGameCountdownText(gameTimer);
    }

    private void EndGame()
    {
        gameEnded = true;
        Debug.Log($"Game Over! Final Score: {totalScore}");
        // Implement additional game over logic here (e.g., show game over screen)
    }

    public void AddScore(int amount, int multiplier = 1)
    {
        if (gameStarted && !gameEnded)
        {
            int addedScore = amount * multiplier;
            totalScore += addedScore;
            currentScore = addedScore;
            UpdateTotalScoreText();
            UpdateCurrentScoreText(amount, multiplier);
            UpdatePowerUpProgress(addedScore);
        }
    }

    private void UpdateCurrentScoreText(int score, int multiplier)
    {
        if (currentScoreText != null)
        {
            Color color = Color.white;
            if (multiplier > 1)
            {
                switch (multiplier)
                {
                    case 2:
                        color = Color.yellow;
                        break;

                    case 3:
                        color = Color.red;
                        break;
                    default:
                        break;
                }

                currentScoreText.text = $"{score} x {multiplier}";
            }
            else
            {
                currentScoreText.text = $"{score}";
            }

            currentScoreText.color = color;
            SetCurrentScoreTextAlpha(1); // Reset alpha to 100%
            currentScoreDisplayTime = currentScoreVisibleDuration; // Reset the display timer

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine); // Stop any active fade coroutine
            }
        }
    }

    private void HandleCurrentScoreTextFade()
    {
        if (currentScoreDisplayTime > 0)
        {
            currentScoreDisplayTime -= Time.deltaTime;
            if (currentScoreDisplayTime <= 0)
            {
                currentScoreDisplayTime = 0;
                fadeCoroutine = StartCoroutine(FadeOutCurrentScoreText());
            }
        }
    }

    private IEnumerator FadeOutCurrentScoreText()
    {
        float elapsedTime = 0;
        Color originalColor = currentScoreText.color;
        while (elapsedTime < currentScoreFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / currentScoreFadeDuration);
            SetCurrentScoreTextAlpha(alpha);
            yield return null;
        }
        SetCurrentScoreTextAlpha(0); // Ensure it is fully transparent at the end
    }

    private void SetCurrentScoreTextAlpha(float alpha)
    {
        if (currentScoreText != null)
        {
            Color color = currentScoreText.color;
            color.a = alpha;
            currentScoreText.color = color;
        }
    }

    private void UpdateTotalScoreText()
    {
        if (totalScoreText != null)
            totalScoreText.text = $"{totalScore}";
    }

    private void UpdatePreGameCountdown()
    {
        preGameTimer -= Time.deltaTime;
        if (preGameTimer <= 0)
        {
            preGameTimer = 0;
            StartCoroutine(StartGame());
        }
        UpdatePreGameCountdownText(preGameTimer);
    }

    private void UpdatePreGameCountdownText(float timer)
    {
        if (preGameCountdownText != null)
        {
            if (timer == 0)
                preGameCountdownText.text = "Begin!";
            else
                preGameCountdownText.text = $"{Mathf.CeilToInt(timer)}";

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
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            int milliseconds = Mathf.FloorToInt((timer % 1) * 100);

            countdownText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }
    }

    public void UsePowerUp()
    {
        if (powerUpCount > 0)
        {
            powerUpCount--;
            UpdatePowerUpIcons();

            var moles = board.MoleHoles;
        }
    }

    private void UpdatePowerUpProgress(int amount)
    {
        if (powerUpCount < 3)
        {
            powerUpProgress += amount;

            if (powerUpProgress >= scoreToUnlockPowerUp)
            {
                powerUpProgress -= scoreToUnlockPowerUp;
                powerUpCount++;
            }

            UpdatePowerUpIcons();
        }
    }

    private void UpdatePowerUpIcons()
    {
        for (int i = 0; i < powerUpIcons.Length; i++)
        {
            if (powerUpIcons[i] != null)
            {
                if (i < powerUpCount)
                    powerUpIcons[i].fillAmount = 1.0f; // Fully filled
                else if (i == powerUpCount)
                    powerUpIcons[i].fillAmount = powerUpProgress / scoreToUnlockPowerUp; // Fill based on progress
                else
                    powerUpIcons[i].fillAmount = 0; // Not filled
            }
        }
    }
}
