using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCharacterActivation : MonoBehaviour
{
    public GameObject[] playerModels;
    private int currentPlayerIndex;
    private string c_SelectedCharacterIndex = "SelectedCharacterIndex";
    void Awake()
    {
        currentPlayerIndex = PlayerPrefs.GetInt(c_SelectedCharacterIndex, 0);

        foreach (GameObject p in playerModels)
        {
            p.SetActive(false);
            playerModels[currentPlayerIndex].SetActive(true);
        }
    }
}
