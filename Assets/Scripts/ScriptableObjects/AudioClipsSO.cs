using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AudioClips")]
public class AudioClipsSO : ScriptableObject
{
    public AudioClip[] steps;
    public AudioClip[] pickUp;
    public AudioClip[] dropDown;
    public AudioClip[] shootBullet;
    public AudioClip[] shootArrow;
    public AudioClip[] swingWeaponHit;
    public AudioClip[] swingWeaponMissed;
}
