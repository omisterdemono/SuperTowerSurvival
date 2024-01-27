using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    [SerializeField] private AudioClipsSO audioClipsSO;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        MeleeWeapon.OnMeleeHit += MeleeWeapon_OnMeleeHit;
        MeleeWeapon.OnMeleeMissed += MeleeWeapon_OnMeleeMissed;
    }

    private void MeleeWeapon_OnMeleeMissed(object sender, System.EventArgs e)
    {
        MeleeWeapon attackerWeapon = sender as MeleeWeapon;
        PlaySound(audioClipsSO.swingWeaponMissed, attackerWeapon.transform.position);
    }

    private void MeleeWeapon_OnMeleeHit(object sender, System.EventArgs e)
    {
        MeleeWeapon attackerWeapon = sender as MeleeWeapon;
        PlaySound(audioClipsSO.swingWeaponHit, attackerWeapon.transform.position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {

        AudioSource.PlayClipAtPoint(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }

    public void PlayFootstepsSound(Vector3 position, float volume)
    {
        PlaySound(audioClipsSO.steps, position, volume);
    }
}
