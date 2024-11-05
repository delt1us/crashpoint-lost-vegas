using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class OptionsMenu : MonoBehaviour
{
    public AudioMixer sfxAudioMixer;
    public AudioMixer musicAudioMixer;

    public CrosshairManager crosshairManager;
    public Image cirlceImageCursor;

    public void SetMusicVolume(float volume)
    {
        musicAudioMixer.SetFloat("Volume", volume);

    }

    public void SetSFXVolume(float volume)
    {
        sfxAudioMixer.SetFloat("SFXVolume", volume);
    }

    public void OnCircleCursorToggleClicked(Toggle toggle)
    {
        if(crosshairManager) crosshairManager.ToggleCircleCursor(toggle.isOn);
        cirlceImageCursor.enabled = toggle.isOn;
    }
}
