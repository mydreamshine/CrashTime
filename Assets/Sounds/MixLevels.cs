using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            pausedText.enabled = !pausedText.enabled;
            Pause();
        }    
    }

    public void Pause()
    {
        // 일시정지
        //Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        Lowpass();
    }

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

    public void ClearVolume()
    {
        masterMixer.ClearFloat("bgmVol");
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
        bgmAudioSource.pitch = Time.timeScale;
    }
}
