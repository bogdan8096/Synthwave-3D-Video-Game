using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }
    private PlayerMotor motor;

    public bool gameHasEnded = false;
    private bool isGameStarted = false;

    private string g_CollectedCoins = "CollectedCurrency";
    private string g_HighScore = "HighScore";

    //menu
    public Text deathScoreText, deathCoinText;
    public GameObject u_GameMenu;
    public GameObject u_DeathMenu;
    public GameObject u_PauseMenu;
    public Button death_Retry;
    public Button death_Menu;

    //ui and ui fields
    private const int coinScoreAmount = 5;
    public Text scoreText, coinText, modifierText;
    private float score, coinScore, modifierScore;
    private int lastScore, collectedCoins, highScore;

    private int coinMultiplier = 1;
    private int scoreMultiplier = 1;

    public int CoinMultiplier { set { coinMultiplier = value; } }
    public int ScoreMultiplier { set { scoreMultiplier = value; } }

    private void Awake()
    {
        Instance = this;
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();

        collectedCoins = PlayerPrefs.GetInt(g_CollectedCoins, 0);
        highScore = PlayerPrefs.GetInt(g_HighScore, 0);

        modifierScore = 1.0f;
        scoreText.text = score.ToString("0");
        coinText.text = coinScore.ToString("0");
        modifierText.text = "x" + modifierScore.ToString("0.0");

        GameObject debugUpdater = GameObject.Find("[Debug Updater]");
        if (debugUpdater != null) Destroy(debugUpdater);
    }

    private void Start()
    {
        StartGame();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.Instance.PauseAudio();
            OnPauseButton();
            u_GameMenu.SetActive(false);
            u_PauseMenu.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AudioManager.Instance.ResumeAudio();
            OnResumeButton();
            u_GameMenu.SetActive(true);
            u_PauseMenu.SetActive(false);
        }

        if (isGameStarted && !gameHasEnded)
        {
            score += (Time.deltaTime * modifierScore * scoreMultiplier);
            
            if(lastScore != (int)score)
            {
                scoreText.text = score.ToString("0");
                lastScore = (int)score;
            }
        }

        if (u_DeathMenu.activeInHierarchy)
        {
            if (GameControllerBluetooth.Instance.GameControllerUpActive) 
            {
                GameControllerBluetooth.Instance.ResetGameControllerInputs();
                death_Menu.onClick.Invoke();
            }
            if (GameControllerBluetooth.Instance.GameControllerDownActive) 
            {
                GameControllerBluetooth.Instance.ResetGameControllerInputs();
                death_Retry.onClick.Invoke();
            }
        }
    }

    public void UpdateModifierScore(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }

    public void UpdateCoinScore()
    {
        coinScore += coinMultiplier;
        coinText.text = coinScore.ToString("0");
        score += coinScoreAmount;
        scoreText.text = score.ToString("0");
    }

    public void OnReplayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        AudioManager.Instance.StopSound("Death");
        u_GameMenu.SetActive(true);
        u_DeathMenu.SetActive(false);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }

    public void OnPauseButton()
    {
        Time.timeScale = 0f;
    }
    public void OnResumeButton()
    {
        Time.timeScale = 1f;
    }

    public void OnMenuButton()
    {
        if (Time.timeScale < 1.0f) Time.timeScale = 1f;

        AudioManager.Instance.StopSound("Game");
        AudioManager.Instance.StopSound("Death");

        u_GameMenu.SetActive(true);
        u_DeathMenu.SetActive(false);

        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }

    public void OnPlayerDeath()
    {
        if(!gameHasEnded)
        {
            gameHasEnded = true;
            FindObjectOfType<BackgroundSpawner>().IsScrolling = false;

            collectedCoins = PlayerPrefs.GetInt(g_CollectedCoins, 0);
            collectedCoins += (int)coinScore;

            PlayerPrefs.SetInt(g_CollectedCoins, collectedCoins);

            if (score > highScore) PlayerPrefs.SetInt(g_HighScore, (int)score);

            deathScoreText.text = score.ToString("0");
            deathCoinText.text = coinScore.ToString("0");

            AudioManager.Instance.PlaySound("Hit");
            AnimationHandler.Instance.TriggerDeathAnimation();

            Invoke("StopGame", 1.0f);
        }
    }

    public void StartGame()
    {
        isGameStarted = true;
        AnimationHandler.Instance.SetAnimatorState(true);
        motor.StartRunning();
        FindObjectOfType<BackgroundSpawner>().IsScrolling = true;
        FindObjectOfType<CameraMotor>().IsPlayerMoving = true;
        AudioManager.Instance.PlaySound("Game");
    }

    public void StopGame()
    {
        u_GameMenu.SetActive(false);
        u_DeathMenu.SetActive(true);
        AudioManager.Instance.StopSound("Game");
        AudioManager.Instance.PlaySound("Death");
        AnimationHandler.Instance.SetAnimatorState(false);
    }
}
