//*************************************************************************************************************
/*  Options menu controller
 *  A script to control the UI in the options menu
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Dean - 16/08/23 - Added mixer functionality
 */
//*************************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class OptionsMenuController : MonoBehaviour
{
    [SerializeField] private CamController camController;
    
    [SerializeField] private AudioMixer sfxMixer;
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private AudioMixer dialogueMixer;
    
    private Button _backButton;
    
    private Slider _masterVolumeSlider;
    private Slider _musicVolumeSlider;
    private Slider _dialogueVolumeSlider;
    private Slider _camSensitivitySlider;
    
    private int _masterVolumeSliderPreviousValue;
    private int _musicVolumeSliderPreviousValue;
    private int _dialogueVolumeSliderPreviousValue;
    private int _camSensitivitySliderPreviousValue;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        _backButton = root.Q<Button>("back-button");
        _backButton.clicked += _GoToMainMenu;
        
        _masterVolumeSlider = root.Q<Slider>("master-volume-slider");
        _masterVolumeSlider.value = PlayerPrefs.GetFloat("master-volume") == 0 ? 15 : PlayerPrefs.GetFloat("master-volume");
        
        _musicVolumeSlider = root.Q<Slider>("music-volume-slider");
        _musicVolumeSlider.value = PlayerPrefs.GetFloat("music-volume") == 0 ? 15 : PlayerPrefs.GetFloat("music-volume");
        
        _dialogueVolumeSlider = root.Q<Slider>("dialogue-volume-slider");
        _dialogueVolumeSlider.value = PlayerPrefs.GetFloat("dialogue-volume") == 0? 15: PlayerPrefs.GetFloat("dialogue-volume");
        
        _camSensitivitySlider = root.Q<Slider>("camera-sensitivity-slider");
        _camSensitivitySlider.value = PlayerPrefs.GetFloat("cam-sensitivity") == 0 ? 75 : PlayerPrefs.GetFloat("cam-sensitivity");
    }

    private void OnDisable()
    {
        _backButton.clicked -= _GoToMainMenu;
    }

    private void Start()
    {
        _masterVolumeSliderPreviousValue = _UpdateSliderPrefs(_masterVolumeSlider, _masterVolumeSliderPreviousValue, "master-volume");
        _musicVolumeSliderPreviousValue = _UpdateSliderPrefs(_musicVolumeSlider, _musicVolumeSliderPreviousValue, "music-volume");
        _dialogueVolumeSliderPreviousValue = _UpdateSliderPrefs(_dialogueVolumeSlider, _dialogueVolumeSliderPreviousValue, "dialogue-volume");
        _camSensitivitySliderPreviousValue = _UpdateSliderPrefs(_camSensitivitySlider, _camSensitivitySliderPreviousValue, "cam_sensitivity", false);
        
        gameObject.SetActive(false);
    }

    private void Update()
    {
        _masterVolumeSliderPreviousValue = _UpdateSliderPrefs(_masterVolumeSlider, _masterVolumeSliderPreviousValue, "master-volume");
        _musicVolumeSliderPreviousValue = _UpdateSliderPrefs(_musicVolumeSlider, _musicVolumeSliderPreviousValue, "music-volume");
        _dialogueVolumeSliderPreviousValue = _UpdateSliderPrefs(_dialogueVolumeSlider, _dialogueVolumeSliderPreviousValue, "dialogue-volume");
        _camSensitivitySliderPreviousValue = _UpdateSliderPrefs(_camSensitivitySlider, _camSensitivitySliderPreviousValue, "cam_sensitivity", false);
    }

    private int _UpdateSliderPrefs(Slider slider, int previousValue, string playerPrefString, bool isVolume = true)
    {
        if ((int)slider.value != previousValue)
        {
            print($"{playerPrefString} value changed");
            PlayerPrefs.SetFloat(playerPrefString, slider.value);
            previousValue = (int)slider.value;

            if (isVolume) _UpdateMusicMixerGroup(playerPrefString);
        }
        return previousValue;
    }

    private void _UpdateMusicMixerGroup(string playerPref)
    {
        float masterVolumeMultiplier = _masterVolumeSlider.value / _masterVolumeSlider.highValue;
        
        float newVolume;
        string mixerGroup;
        AudioMixer audioMixer;
        switch (playerPref)
        {
            case "master-volume":
                mixerGroup = "SFX Vol";
                newVolume = _masterVolumeSlider.value - 20;
                audioMixer = sfxMixer;
                _musicVolumeSliderPreviousValue = _UpdateSliderPrefs(_musicVolumeSlider, 9999, "music-volume");
                _dialogueVolumeSliderPreviousValue = _UpdateSliderPrefs(_dialogueVolumeSlider, 9999, "dialogue-volume");
                break;
            case "music-volume":
                mixerGroup = "MusicVol";
                newVolume = _musicVolumeSlider.value * masterVolumeMultiplier - 20;
                audioMixer = musicMixer;
                break;
            case "dialogue-volume":
                mixerGroup = "DialogueVol";
                newVolume = _dialogueVolumeSlider.value * masterVolumeMultiplier - 20;
                audioMixer = dialogueMixer;
                break;
            default:
                throw new Exception("player pref not added to case switch in options menu controller");
        }
        
        audioMixer.SetFloat(mixerGroup, newVolume);
    }
    
    private void _GoToMainMenu()
    {
        camController.SetCameraDestinationMainMenu();
        gameObject.SetActive(false);
    }
}
