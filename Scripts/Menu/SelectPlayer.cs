using UnityEngine;
using UnityEngine.UI;

public class SelectPlayer : MonoBehaviour
{
    public GameObject[] playerModels;
    public PlayerBlueprint[] players;
    public int currentPlayerIndex;
    public Button playButton;

    private string c_CollectedCurrency = "CollectedCurrency";
    private string c_HighScore = "HighScore";

    public Text startPlayerName, highScoreText, collectedCoinsText;

    private string c_SelectedCharacterIndex = "SelectedCharacterIndex";

    private void Start()
    {
        UpdateLogs();
        GetCharacterSettings();
    }

    public void ChangeNextPlayer()
    {
        playerModels[currentPlayerIndex].SetActive(false);
        currentPlayerIndex++;

        if (currentPlayerIndex == playerModels.Length)
            currentPlayerIndex = 0;
        
        playerModels[currentPlayerIndex].SetActive(true);

        UnlockCharacter();

        PlayerPrefs.SetInt(c_SelectedCharacterIndex, currentPlayerIndex);
    }

    public void ChangePreviousPlayer()
    {
        playerModels[currentPlayerIndex].SetActive(false);
        currentPlayerIndex--;

        if (currentPlayerIndex == -1)
            currentPlayerIndex = playerModels.Length - 1;

        playerModels[currentPlayerIndex].SetActive(true);

        UnlockCharacter();

        PlayerPrefs.SetInt(c_SelectedCharacterIndex, currentPlayerIndex);
    }

    public void UnlockCharacter()
    {
        PlayerBlueprint player = players[currentPlayerIndex];
        startPlayerName.text = player.name;
        if (!player.isUnlocked)
        {
            playButton.gameObject.SetActive(false);
            return;
        }
        else playButton.gameObject.SetActive(true);
    }

    public void GetCharacterSettings()
    {
        foreach (PlayerBlueprint player in players)
        {
            if (player.price == 0)
                player.isUnlocked = true;
            else
                player.isUnlocked = PlayerPrefs.GetInt(player.name, 0) == 0 ? false : true;
        }

        currentPlayerIndex = PlayerPrefs.GetInt(c_SelectedCharacterIndex, 0);
        startPlayerName.text = players[currentPlayerIndex].name;

        foreach (GameObject p in playerModels)
        {
            p.SetActive(false);
            playerModels[currentPlayerIndex].SetActive(true);
        }
    }

    public void OnResetSelected()
    {
        UpdateLogs();
        currentPlayerIndex = 0;

        foreach (PlayerBlueprint player in players)
        {
            if (player.price == 0) continue;
            player.isUnlocked = false;
            PlayerPrefs.SetInt(player.name, 0);
        }

        UnlockCharacter();
    }

    public void UpdateLogs()
    {
        highScoreText.text = PlayerPrefs.GetInt(c_HighScore).ToString();
        collectedCoinsText.text = PlayerPrefs.GetInt(c_CollectedCurrency).ToString();
    }
    /*
    public void OnExitDisable()
    {
        playerModels[currentPlayerIndex].SetActive(false);
    }
    public void OnLoadEnable()
    {
        UnlockCharacter();
        Debug.Log(currentPlayerIndex);
        playerModels[currentPlayerIndex].SetActive(true);
    }
    */
}
