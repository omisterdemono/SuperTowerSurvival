using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightSound : MonoBehaviour
{
    [SerializeField] private AudioClipsSO audioClipsSO;
    [SerializeField] private float daySoundsVolume = .1f;
    [SerializeField] private float nightSoundsVolume = .3f;
    [SerializeField] private float GeneralVolume = 1.0f;
    [SerializeField] private float SfxVolume = 1.0f;

    private AudioSource audioSource;
    private AudioClip nightSound;
    private AudioClip daySound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        daySound = audioClipsSO.daySound;
        nightSound = audioClipsSO.nightSound;
    }

    private void Start()
    {
        audioSource.clip = daySound;
        audioSource.volume = daySoundsVolume;
        audioSource.Play();
        WorldLight.OnNightChanged += WorldLight_OnNightChanged;
        SoundOptions.OnGeneralVolumeChange += SoundOptions_OnGeneralVolumeChange;
        SoundOptions.OnSfxVolumeChange += SoundOptions_OnSfxVolumeChange; ;
    }
    private void SoundOptions_OnGeneralVolumeChange(object sender, float e)
    {
        GeneralVolume = (float)e;
        SetAudioVolume();
    }
    private void SoundOptions_OnSfxVolumeChange(object sender, float e)
    {
        SfxVolume = (float)e;
        SetAudioVolume();
    }

    private void SetAudioVolume()
    {
        audioSource.volume = daySoundsVolume * GeneralVolume * SfxVolume;
    }

    private void WorldLight_OnNightChanged(object sender, System.EventArgs e)
    {
        WorldLight worldLight = (WorldLight)sender;
        if (worldLight.isNight)
        {
            audioSource.clip = nightSound;
            audioSource.volume = nightSoundsVolume * GeneralVolume * SfxVolume;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = daySound;
            audioSource.volume = daySoundsVolume * GeneralVolume * SfxVolume;
            audioSource.Play();
        }
    }
}
