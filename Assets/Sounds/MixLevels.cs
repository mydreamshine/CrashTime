using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixLevels : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Text pausedText;
    public AudioMixerSnapshot paused;
    public AudioMixerSnapshot unpaused;
    public AudioSource bgmAudioSource;

    public void Lowpass()
    {
        if (pausedText.enabled)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            paused.TransitionTo(0.01f);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            unpaused.TransitionTo(0.01f);
        }
    }
    
    public void SetSfxLvl(float sfxLvl)
    {
        masterMixer.SetFloat("sfxVol", sfxLvl);
    }

    public void SetBgmLvl(float bgmLvl)
    {
        masterMixer.SetFloat("bgmVol", bgmLvl);
    }

    public float GetSfxLvl()
    {
        masterMixer.GetFloat("sfxVol", out var value);
        return value;
    }
    
    public float GetBgmLvl()
    {
        masterMixer.GetFloat("bgmVol", out var value);
        return value;
    }

    public void ClearBgmVolume()
    {
        masterMixer.ClearFloat("bgmVol");
    }
    public void ClearSfxVolume()
    {
        masterMixer.ClearFloat("sfxVol");
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
        bgmAudioSource.pitch = Time.timeScale;
    }
}
