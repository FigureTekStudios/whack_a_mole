using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsPaused { get => isPaused; }
    public bool GameStarted { get => gameStarted; }
    public bool GameEnded { get => gameEnded; }
    
    public int PowerUpCount { get => powerUpCount; }

    public int preGameCountdownTime = 3; // pre-game countdown time in seconds
    public int initialGameTime = 103; // this should be determined by song length
    public int scoreToUnlockPowerUp = 100; // Score needed to unlock a power-up

    public GameObject hudPanel;
    public GameObject startGameMenuPanel;
    public GameObject endGameMenuPanel;
    public GameObject pauseMenuPanel;

    public TextMeshProUGUI preGameCountdownText;
    public TextMeshProUGUI countdownText; 
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI finalScoreText;

    private bool powerUpEnabled = false;
    public Image[] powerUpIcons; // UI Images to display power-up icons

    // these are references to objects in the second monitor
    public TextMeshProUGUI preGameCountdownText1;
    public TextMeshProUGUI countdownText1;
    public TextMeshProUGUI currentScoreText1;
    public TextMeshProUGUI totalScoreText1;
    public TextMeshProUGUI finalScoreText1;

    public Image[] powerUpIcons1; // UI Images to display power-up icons

    private GameBoard board; // this should probably be another singleton
    private List<GameObject> moleHoles;
    public List<GameObject> MoleHoles { get => moleHoles; }
    public bool PowerUpEnabled { get => powerUpEnabled; }

    private int currentScore;
    private int totalScore;
    private float gameTimer;
    private float preGameTimer;
    private bool gameStarted;
    private bool gameEnded;
    private bool isPaused;
    private int powerUpCount;
    private float powerUpProgress;
    
    
    private Coroutine fadeCoroutine;
    private float currentScoreDisplayTime;
    private float currentScoreFadeDuration = 1;
    private float currentScoreVisibleDuration = 1f;

    [SerializeField] GameObject electrictyParticles;
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

        if (Conductor.Instance != null)
        {
            initialGameTime = (int) Math.Ceiling(Conductor.Instance.song.songLength); ;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            StartGame();
        if (Input.GetKeyDown(KeyCode.R))
            RestartGame();

        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(UsePowerUp());


        if (Input.GetKeyDown(KeyCode.P) && gameStarted)
        {
            TogglePause();
        }

        if (!gameStarted)
        {
            if (gameEnded || isPaused)
            {
                return;
            }
        }
        else if (!gameEnded && !isPaused)
        {
            UpdateGameCountdown();
            HandleCurrentScoreTextFade();
        }
    }

    private void InitializeGame()
    {
        StartCoroutine(board.GenerateGameBoard());
        moleHoles = board.MoleHoles;

        totalScore = 0;
        currentScore = 0;
        gameTimer = initialGameTime;
        preGameTimer = preGameCountdownTime;
        gameStarted = false;
        gameEnded = false;
        powerUpCount = 0;
        powerUpProgress = 0;
        currentScoreDisplayTime = 0;
        UpdateCurrentScoreText(0, 1);
        UpdateTotalScoreText();
        SetCurrentScoreTextAlpha(0); // Start with current score text hiddenUpdateCurrentScoreText(0, 1);
        UpdatePreGameCountdownText(preGameTimer); // starts the game basically

        UpdatePowerUpIcons();
 
        endGameMenuPanel.SetActive(false);
        hudPanel.SetActive(false); 
        pauseMenuPanel.SetActive(false); 
        startGameMenuPanel.SetActive(true);    
    }

    public void StartGame()
    {
        startGameMenuPanel.SetActive(false);
        hudPanel.SetActive(true);
        //yield return new WaitForSeconds(1f);
        gameStarted = false;
        preGameCountdownText.gameObject.SetActive(true); // Hide the pre-game countdown text
        preGameCountdownText1.gameObject.SetActive(true); // Hide the pre-game countdown text
        
        StartCoroutine(PreGameCountdownCoroutine());
    }

    private IEnumerator PreGameCountdownCoroutine()
    {
        while (preGameTimer >= 0)
        {
            UpdatePreGameCountdownText(preGameTimer);
            yield return new WaitForSeconds(1f);
            preGameTimer--;
        }

        StartActualGame();
    }

    private void StartActualGame()
    {
        gameStarted = true;
        Conductor.Instance.PlaySong();

        preGameCountdownText.gameObject.SetActive(false); // Hide the pre-game countdown text
        preGameCountdownText1.gameObject.SetActive(false); // Hide the pre-game countdown text

        countdownText.gameObject.SetActive(true);
        countdownText1.gameObject.SetActive(true);

        totalScoreText.gameObject.SetActive(true);
        totalScoreText1.gameObject.SetActive(true);
        UpdateGameCountdownText(gameTimer);
    }

    private void EndGame()
    {
        gameEnded = true;
        Debug.Log($"Game Over! Final Score: {totalScore}");
        hudPanel.gameObject.SetActive(false);   
        endGameMenuPanel.gameObject.SetActive(true);
        finalScoreText.text = totalScore.ToString();
        finalScoreText1.text = finalScoreText.text;
        // Implement additional game over logic here (e.g., show game over screen)
        Conductor.Instance.StopSong();
        board.RetreatAllMoleHoles();
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


            if (currentScoreText1 != null)
            {
                currentScoreText1.text = currentScoreText.text;
                currentScoreText1.color = currentScoreText.color;
            }

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
            currentScoreText1.color = color;
        }
    }

    private void UpdateTotalScoreText()
    {
        if (totalScoreText != null)
            totalScoreText.text = $"{totalScore}";

        if (totalScoreText1 != null)
            totalScoreText1.text = $"{totalScore}";
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
        {
            if (timer == 0)
                preGameCountdownText.text = "Begin!";
            else
                preGameCountdownText.text = $"{Mathf.CeilToInt(timer)}";

        }

        if (preGameCountdownText1 != null)
            preGameCountdownText1.text = preGameCountdownText.text;
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
            countdownText1.text = countdownText.text;
        }
    }

    public IEnumerator UsePowerUp()
    {

        if (powerUpCount > 0)
        {
            powerUpEnabled = true;
            electrictyParticles.SetActive(powerUpEnabled);
            SoundManager.Instance.PlayOnUsePowerUpVO();
            powerUpCount--;
            UpdatePowerUpIcons();

            var moles = moleHoles
            .Select(moleHoleObj => moleHoleObj.GetComponent<MoleHole>())
            .Where(moleHole => moleHole != null &&
                               moleHole.State != MoleState.hiding &&
                               moleHole.State != MoleState.shocked &&
                               moleHole.State != MoleState.hit)
            .ToList();

            foreach (var mole in moles) { StartCoroutine(mole.Shock()); }
            yield return new WaitForSeconds(3f);
        }
        else { yield return null; }
        powerUpEnabled = false;
        electrictyParticles.SetActive(powerUpEnabled);
    }

    private void UpdatePowerUpProgress(int amount)
    {
        if (powerUpCount < 3)
        {
            powerUpProgress += amount;

            if (powerUpProgress >= scoreToUnlockPowerUp)
            {
                powerUpProgress -= scoreToUnlockPowerUp;
                SoundManager.Instance.PlayOnObtainedPowerUpSFX();
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

            if (powerUpIcons1[i] != null)
                powerUpIcons1[i].fillAmount = powerUpIcons[i].fillAmount;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        hudPanel.SetActive(!isPaused);
        pauseMenuPanel.SetActive(isPaused);
        
        if (isPaused)
        {
            Conductor.Instance.PauseSong();
        }
        else
        {
            Conductor.Instance.ResumeSong();
        }
    }

    public void RestartGame()
    {
        Conductor.Instance.StopSong();
        StopAllCoroutines();
        board.DeleteAllMoleHoles();
        InitializeGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
