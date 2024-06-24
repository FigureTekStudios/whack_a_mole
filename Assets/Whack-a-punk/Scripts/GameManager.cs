using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int initialCountdownTime = 60; // this should be determined by song length
    public TextMeshProUGUI countdownText; 
    public TextMeshProUGUI scoreText; 

    private int score;
    private float countdownTimer;
    private bool gameEnded;

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
    }

    private void Start()
    {
        InitializeGame();
    }

    private void Update()
    {
        if (!gameEnded)
        {
            UpdateCountdown();
        }
    }

    public void AddScore(int amount)
    {
        if (!gameEnded)
        {
            score += amount;
            UpdateScoreText();
        }
    }

    private void InitializeGame()
    {
        score = 0;
        countdownTimer = initialCountdownTime;
        gameEnded = false;
        UpdateScoreText();
        UpdateCountdownText();
    }

    private void UpdateCountdown()
    {
        countdownTimer -= Time.deltaTime;
        if (countdownTimer <= 0)
        {
            countdownTimer = 0;
            EndGame();
        }
        UpdateCountdownText();
    }

    private void EndGame()
    {
        gameEnded = true;
        Debug.Log("Game Over! Final Score: " + score);
        // Implement additional game over logic here (e.g., show game over screen)
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void UpdateCountdownText()
    {
        if (countdownText != null)
        {
            countdownText.text = "Time: " + Mathf.CeilToInt(countdownTimer);
        }
    }
}
