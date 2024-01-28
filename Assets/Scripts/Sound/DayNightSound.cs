using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightSound : MonoBehaviour
{
    [SerializeField] private AudioClipsSO audioClipsSO;
    [SerializeField] private float daySoundsVolume = .1f;
    [SerializeField] private float nightSoundsVolume = .3f;
    
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
    }

    private void WorldLight_OnNightChanged(object sender, System.EventArgs e)
    {
        WorldLight worldLight = (WorldLight)sender;
        if (worldLight.isNight)
        {
            audioSource.clip = nightSound;
            audioSource.volume = nightSoundsVolume;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = daySound;
            audioSource.volume = daySoundsVolume;
            audioSource.Play();
        }
    }
}
