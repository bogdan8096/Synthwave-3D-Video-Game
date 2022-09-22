using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private string s_FirstPlay = "FirstPlay";
    private string s_BackgroundMusicPref = "BackgroundMusic";
    private string s_SoundEffectsPref = "SoundEffects";

    private int s_FirstPlayInt;
    public Slider s_BackgroudMusicSlider, s_SoundEffectsSlider;
    private float s_BackgroundMusicFloat, s_SoundEffectsFloat;

    public AudioSource s_BackgroundAudio;

    private string c_BuyCharacterIndex = "BuyCharacterIndex";
    private string c_CollectedCurrency = "CollectedCurrency";
    private string c_HighScore = "HighScore";

    public int currentPlayerIndex;
    public GameObject[] playerModels;
    public PlayerBlueprint[] players;
    public Button buyButton;
    public Text priceText, shopCurrencyText, playerName;

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        GameObject debugUpdater = GameObject.Find("[Debug Updater]");
        if (debugUpdater != null) Destroy(debugUpdater);
    }
    // Start is called before the first frame update
    void Start()
    {
        GetSoundSettings();
        GetPlayerSettings();
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            UpdateBackgroundSound();
            UpdateEffectsound();
        }
    }

    public void UpdateBackgroundSound()
    {
        s_BackgroundAudio.volume = s_BackgroudMusicSlider.value;
        PlayerPrefs.SetFloat(s_BackgroundMusicPref, s_BackgroudMusicSlider.value);
        PlayerPrefs.Save();
    }
    public void UpdateEffectsound()
    {
        PlayerPrefs.SetFloat(s_SoundEffectsPref, s_SoundEffectsSlider.value);
        PlayerPrefs.Save();
    }

    public void GetSoundSettings()
    {
        s_FirstPlayInt = PlayerPrefs.GetInt(s_FirstPlay);

        if (s_FirstPlayInt == 0)
        {
            PlayerPrefs.DeleteAll();

            s_BackgroundMusicFloat = 1.0f;
            s_SoundEffectsFloat = 1.0f;

            s_BackgroudMusicSlider.value = s_BackgroundMusicFloat;
            s_SoundEffectsSlider.value = s_SoundEffectsFloat;

            PlayerPrefs.SetFloat(s_BackgroundMusicPref, s_BackgroundMusicFloat);
            PlayerPrefs.SetFloat(s_SoundEffectsPref, s_SoundEffectsFloat);

            PlayerPrefs.SetInt(s_FirstPlay, -1);
        }
        else
        {
            s_BackgroundMusicFloat = PlayerPrefs.GetFloat(s_BackgroundMusicPref);
            s_BackgroudMusicSlider.value = s_BackgroundMusicFloat;

            s_SoundEffectsFloat = PlayerPrefs.GetFloat(s_SoundEffectsPref);
            s_SoundEffectsSlider.value = s_SoundEffectsFloat;
        }
    }

    public void GetPlayerSettings()
    {
        foreach (PlayerBlueprint player in players)
        {
            if (player.price == 0)
                player.isUnlocked = true;
            else
                player.isUnlocked = PlayerPrefs.GetInt(player.name, 0) == 0 ? false : true;
        }

        currentPlayerIndex = PlayerPrefs.GetInt(c_BuyCharacterIndex, 0);

        foreach (GameObject p in playerModels)
        {
            p.SetActive(false);
            playerModels[currentPlayerIndex].SetActive(true);
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        PlayerBlueprint player = players[currentPlayerIndex];

        shopCurrencyText.text = PlayerPrefs.GetInt(c_CollectedCurrency).ToString();

        playerName.text = player.name;

        if (player.isUnlocked)
        {
            buyButton.gameObject.SetActive(false);
            priceText.text = "";
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            priceText.text = player.price.ToString();
            if (PlayerPrefs.GetInt(c_CollectedCurrency) >= player.price)
            {
                buyButton.interactable = true;
            }
            else
            {
                buyButton.interactable = false;
            }
        }
    }

    public void NextPlayerToBuy()
    {
        playerModels[currentPlayerIndex].SetActive(false);
        currentPlayerIndex++;

        if (currentPlayerIndex == playerModels.Length)
            currentPlayerIndex = 0;

        playerModels[currentPlayerIndex].SetActive(true);

        UpdateUI();

        PlayerBlueprint player = players[currentPlayerIndex];
        if (!player.isUnlocked) return;

        PlayerPrefs.SetInt(c_BuyCharacterIndex, currentPlayerIndex);
    }

    public void PreviousPlayerToBuy()
    {
        playerModels[currentPlayerIndex].SetActive(false);
        currentPlayerIndex--;

        if (currentPlayerIndex == -1)
            currentPlayerIndex = playerModels.Length - 1;

        playerModels[currentPlayerIndex].SetActive(true);

        UpdateUI();

        PlayerBlueprint player = players[currentPlayerIndex];
        if (!player.isUnlocked) return;

        PlayerPrefs.SetInt(c_BuyCharacterIndex, currentPlayerIndex);
    }

    public void OnBuyButton()
    {
        PlayerBlueprint player = players[currentPlayerIndex];

        PlayerPrefs.SetInt(player.name, 1);
        PlayerPrefs.SetInt(c_BuyCharacterIndex, currentPlayerIndex);
        player.isUnlocked = true;
        PlayerPrefs.SetInt(c_CollectedCurrency, PlayerPrefs.GetInt(c_CollectedCurrency, 0) - player.price);

        UpdateUI();
    }

    public void OnCheatMode()
    {
        int amountCurrencyIncrease = 69420;
        PlayerPrefs.SetInt(c_CollectedCurrency, amountCurrencyIncrease);
        shopCurrencyText.text = PlayerPrefs.GetInt(c_CollectedCurrency).ToString();
    }

    public void OnResetDefaults()
    {
        int amountCurrencyReset = 0;
        int amountHighScoreReset = 0;
        PlayerPrefs.SetInt(c_CollectedCurrency, amountCurrencyReset);
        PlayerPrefs.SetInt(c_HighScore, amountHighScoreReset);
        shopCurrencyText.text = PlayerPrefs.GetInt(c_CollectedCurrency).ToString();

        PlayerPrefs.SetInt("BuyCharacterIndex", 0); 
        PlayerPrefs.SetInt("SelectedCharacterIndex", 0);

        foreach (PlayerBlueprint player in players)
        {
            player.isUnlocked = false;
            PlayerPrefs.SetInt(player.name, 0);
        }
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
