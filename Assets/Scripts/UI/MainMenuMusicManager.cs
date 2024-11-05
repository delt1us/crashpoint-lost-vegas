using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMusicManager : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.Play();
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        _audioSource.Stop();
    }
}
