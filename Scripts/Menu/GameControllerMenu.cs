using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerMenu : MonoBehaviour
{
    public GameObject startMenuObject;
    public GameObject settingsMenuObject;
    public GameObject shopMenuObject;

    public Button start_PreviusButton;
    public Button start_NextButton;
    public Button start_StartButton;
    public Button start_SettingsButton;
    public Button start_ShopButton;

    public Button settings_Cheat;
    public Button settings_Reset;
    public Button settings_Back;

    public Button shop_PreviusButton;
    public Button shop_NextButton;
    public Button shop_Buy;
    public Button shop_Back;

    private bool isLeftButtonPressed    = false;
    private bool isRightButtonPressed   = false;
    private bool isUpButtonPressed      = false;
    private bool isDownButtonPressed    = false;

    // Update is called once per frame
    void Update()
    {
        if (startMenuObject.activeInHierarchy)
        {
            if (GameControllerBluetooth.Instance.GameControllerLeftActive && !isLeftButtonPressed)
            {
                GameControllerBluetooth.Instance.ResetGameControllerInputs();
                isLeftButtonPressed = true;
                start_PreviusButton.onClick.Invoke();
            }
            else if (GameControllerBluetooth.Instance.GameControllerRightActive && !isRightButtonPressed)
            {
                GameControllerBluetooth.Instance.ResetGameControllerInputs();
                isRightButtonPressed = true;
                start_NextButton.onClick.Invoke();
            }
            else if (GameControllerBluetooth.Instance.GameControllerUpActive && !isUpButtonPressed) 
            {
                GameControllerBluetooth.Instance.ResetGameControllerInputs();
                isUpButtonPressed = true;
                start_StartButton.onClick.Invoke(); 
            }
            else if (GameControllerBluetooth.Instance.GameControllerDownActive && !isDownButtonPressed) 
            {
                GameControllerBluetooth.Instance.ResetGameControllerInputs();
                isDownButtonPressed = true;
                start_ShopButton.onClick.Invoke(); 
            }
            else
            {
                isLeftButtonPressed     = false;
                isRightButtonPressed    = false;
                isUpButtonPressed       = false;
                isDownButtonPressed     = false;
            }
        }
        else if (settingsMenuObject.activeInHierarchy)
        {
            if (GameControllerBluetooth.Instance.GameControllerLeftActive && !isLeftButtonPressed) 
            {
                GameControllerBluetooth.Instance.ResetGameControllerInputs();
                isLeftButtonPressed = true;
                settings_Cheat.onClick.Invoke(); 
            }
            else if (GameControllerBluetooth.Instance.GameControllerRightActive && !isRightButtonPressed) 
            {
                GameControllerBluetooth.Instance.ResetGameControllerInputs();
                isRightButtonPressed = true;
                settings_Reset.onClick.Invoke(); 
            }
            else if (GameControllerBluetooth.Instance.GameControllerUpActive && !isUpButtonPressed) 
            {
                GameControllerBluetooth.Instance.ResetGameControllerInputs();
                isUpButtonPressed = true;
                settings_Back.onClick.Invoke(); 
            }
            else
            {
                isLeftButtonPressed = false;
                isRightButtonPressed = false;
                isUpButtonPressed = false;
                isDownButtonPressed = false;
            }
        }
        else if (shopMenuObject.activeInHierarchy)
        {
            if (GameControllerBluetooth.Instance.GameControllerLeftActive &&!isLeftButtonPressed) 
            {
                GameControllerBluetooth.Instance.ResetGameControllerInputs();
                isLeftButtonPressed = true;
                shop_PreviusButton.onClick.Invoke(); 
            }
            else if (GameControllerBluetooth.Instance.GameControllerRightActive && !isRightButtonPressed) 
            {
                GameControllerBluetooth.Instance.ResetGameControllerInputs();
                isRightButtonPressed = true;
                shop_NextButton.onClick.Invoke(); 
            }
            else if (GameControllerBluetooth.Instance.GameControllerUpActive && !isUpButtonPressed) 
            {
                GameControllerBluetooth.Instance.ResetGameControllerInputs();
                isUpButtonPressed = true;
                shop_Buy.onClick.Invoke(); 
            }
            else if (GameControllerBluetooth.Instance.GameControllerDownActive && !isDownButtonPressed) 
            {
                GameControllerBluetooth.Instance.ResetGameControllerInputs();
                isDownButtonPressed = true;
                shop_Back.onClick.Invoke(); 
            }
            else
            {
                isLeftButtonPressed = false;
                isRightButtonPressed = false;
                isUpButtonPressed = false;
                isDownButtonPressed = false;
            }
        }
    }
}
