using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private float MusicVolumeOption = 1.0f;
    [SerializeField] private float GeneralVolumeOption = 1.0f;
    [SerializeField] private float basicMusicVolume = 0.5f;
    AudioSource musicSource;

    void Start()
    {
        SoundOptions.OnMusicVolumeChange += SoundOptions_OnMusicVolumeChange;
        SoundOptions.OnGeneralVolumeChange += SoundOptions_OnGeneralVolumeChange; ;
        musicSource = GetComponent<AudioSource>();
    }

    private void SoundOptions_OnGeneralVolumeChange(object sender, float e)
    {
        GeneralVolumeOption = (float)e;
        setBGMusicVolume();
    }

    private void SoundOptions_OnMusicVolumeChange(object sender, float e)
    {
        MusicVolumeOption = (float)e;
        setBGMusicVolume();
    }
    private void setBGMusicVolume()
    {
        musicSource.volume = MusicVolumeOption * GeneralVolumeOption * basicMusicVolume;
    }
}
