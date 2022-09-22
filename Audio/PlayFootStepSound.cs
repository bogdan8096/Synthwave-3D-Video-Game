using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFootStepSound : MonoBehaviour
{
    void PlayStepSound()
    {
        FindObjectOfType<AudioManager>().PlaySound("Step");
    }

    void PlayLeftStepSound()
    {
        FindObjectOfType<AudioManager>().PlaySound("LeftStep");
    }

    void PlayRightStepSound()
    {
        FindObjectOfType<AudioManager>().PlaySound("RightStep");
    }
}