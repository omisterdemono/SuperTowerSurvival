using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOptions : MonoBehaviour
{
    [SerializeField] private float GeneralVolume = 1.0f;
    [SerializeField] private float SfxVolume = 1.0f;
    [SerializeField] private float MusicVolume = 1.0f;

    public static event EventHandler<float> OnGeneralVolumeChange;
    public static event EventHandler<float> OnSfxVolumeChange;
    public static event EventHandler<float> OnMusicVolumeChange;


    public void SetGeneralVolume(float value)
    {
        GeneralVolume = value;
        OnGeneralVolumeChange?.Invoke(this, value);
    }
    public void SetSfxVolume(float value)
    {
        SfxVolume = value;
        OnSfxVolumeChange?.Invoke(this, value);
    }
    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
        OnMusicVolumeChange?.Invoke(this, value);
    }
}
