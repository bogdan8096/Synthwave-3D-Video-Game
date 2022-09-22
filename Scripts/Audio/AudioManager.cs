using UnityEngine.UI;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { set; get; }

    public Sound[] sounds;

    private string s_BackgroundMusicPref = "BackgroundMusic";
    private string s_SoundEffectsPref = "SoundEffects";
    private float s_BackgroundMusicFloat, s_SoundEffectsFloat;

    private PlayerMotor motor;

    private void Awake()
    {
        Instance = this;

        s_BackgroundMusicFloat = PlayerPrefs.GetFloat(s_BackgroundMusicPref);
        s_SoundEffectsFloat = PlayerPrefs.GetFloat(s_SoundEffectsPref);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            if (s.name == "Game" || s.name == "Death")
            {
                s.source.volume = s_BackgroundMusicFloat;
            }
            else
            {
                s.source.volume = s_SoundEffectsFloat;
            }
            s.source.loop = s.loop;
        }
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
    }

    public void PlaySound(string name)
    {
        foreach(Sound s in sounds)
        {
            if(s.name == name)
            {
                s.source.Play();
            }
        }
    }

    public void StopSound(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                s.source.Stop();
            }
        }
    }

    public void PauseAudio()
    {
        AnimationHandler.Instance.SetPauseAnimationSpeed(true);
        foreach (Sound s in sounds)
        {
            if(s.name == "Game")
            {
                s.source.volume = 0.15f;
                s.source.pitch = 0.5f;
            }
            else
            {
                s.source.volume = 0f;
            }
        }
    }

    public void ResumeAudio()
    {
        AnimationHandler.Instance.SetPauseAnimationSpeed(false);
        foreach (Sound s in sounds)
        {
            if (s.name == "Game" || s.name == "Death" || s.name == "Menu")
            {
                s.source.volume = s_BackgroundMusicFloat;
            }
            else
            {
                s.source.volume = s_SoundEffectsFloat;
            }
            s.source.pitch = 1f;
        }
    }
}
