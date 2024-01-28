using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AudioClips")]
public class AudioClipsSO : ScriptableObject
{
    public AudioClip[] steps;
    public AudioClip[] shootRifle;
    public AudioClip[] shootShotgun;
    public AudioClip[] meleeWeaponHit;
    public AudioClip[] meleeWeaponMissed;

    public AudioClip[] chargedWeaponSwing;
    public AudioClip[] chargedWeaponSpin;
    
    public AudioClip[] zombieHit;
    public AudioClip[] skeletonHit;
    public AudioClip[] playerHit;

    public AudioClip arrowHit;
    public AudioClip bulletHit;
    public AudioClip releaseBow;
    public AudioClip loadBow;
    public AudioClip pickUp;
    public AudioClip dropDown;
    public AudioClip itemEquip;
}
