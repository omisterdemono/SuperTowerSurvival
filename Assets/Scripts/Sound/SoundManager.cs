using Mirror;
using Mirror.Examples.Tanks;
using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    [SerializeField] private AudioClipsSO audioClipsSO;
    [SerializeField] private float GeneralVolume = 1.0f;
    [SerializeField] private float SfxVolume = 1.0f;
    //[SerializeField] private float MusicVolumeOption = 1.0f;

    //public void SetGeneralVolume(float value)
    //{
    //    GeneralVolume = value;
    //}

    //public void SetSfxlVolume(float value)
    //{
    //    SfxVolume = value;
    //}

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        MeleeWeapon.OnMeleeHit += MeleeWeapon_OnMeleeHit;
        MeleeWeapon.OnMeleeMissed += MeleeWeapon_OnMeleeMissed;

        ChargeMeleeWeapon.OnMeleeSpin += ChargeMeleeWeapon_OnMeleeSpin;
        ChargeMeleeWeapon.OnMeleeSwing += ChargeMeleeWeapon_OnMeleeSwing;

        Bow.OnBowReleased += Bow_OnBowRelease;
        FireWeapon.OnShoot += FireWeapon_OnShoot;
        Projectile.OnAnyProjectileHit += Projectile_OnProjectileHit;

        HealthComponent.OnEntityHit += HealthComponent_OnEntityHit;

        SoundOptions.OnGeneralVolumeChange += SoundOptions_OnGeneralVolumeChange;
        SoundOptions.OnSfxVolumeChange += SoundOptions_OnSfxVolumeChange; ;
        SoundOptions.OnMusicVolumeChange += SoundOptions_OnMusicVolumeChange; ;
    }
    private void SoundOptions_OnGeneralVolumeChange(object sender, float e)
    {
        GeneralVolume = (float)e;
    }
    private void SoundOptions_OnSfxVolumeChange(object sender, float e)
    {
        SfxVolume = (float)e;
    }
    private void SoundOptions_OnMusicVolumeChange(object sender, float e)
    {
        //mus = (float)e;
    }

    private void ChargeMeleeWeapon_OnMeleeSwing(object sender, System.EventArgs e)
    {
        ChargeMeleeWeapon chargedMelee = sender as ChargeMeleeWeapon;
        PlaySound(audioClipsSO.chargedWeaponSwing, chargedMelee.transform.position);
    }

    private void ChargeMeleeWeapon_OnMeleeSpin(object sender, System.EventArgs e)
    {
        ChargeMeleeWeapon chargedMelee = sender as ChargeMeleeWeapon;
        PlaySound(audioClipsSO.chargedWeaponSpin, chargedMelee.transform.position);
    }

    private void HealthComponent_OnEntityHit(object sender, System.EventArgs e)
    {
        HealthComponent health = (HealthComponent)sender;
        var position = health.transform.position;
        var type = health._entityType;
        if (type == HealthComponent.EntityType.Player)
        {
            //PlaySound(audioClipsSO.playerHit, Camera.main.transform.position, 0.7f);
            PlaySound(audioClipsSO.playerHit, position, 0.7f);
        }
        else if (type == HealthComponent.EntityType.Zombie)
        {
            PlaySound(audioClipsSO.zombieHit, position, 0.7f);
        }
        else if (type == HealthComponent.EntityType.Skeleton)
        {
            PlaySound(audioClipsSO.skeletonHit, position, 0.7f);
        }
        else if (type == HealthComponent.EntityType.Structure)
        {
            //PlaySound(audioClipsSO.structureHit, position, 0.7f);
        }
    }

    private void FireWeapon_OnShoot(object sender, System.EventArgs e)
    {
        FireWeapon weapon = (FireWeapon)sender;
        var position = weapon.transform.position;
        var type = weapon.type;
        if (type == FireWeapon.Type.Shotgun)
        {
            PlaySound(audioClipsSO.shootShotgun, position, 0.7f);
        }
        else if (type == FireWeapon.Type.Rifle)
        {
            PlaySound(audioClipsSO.shootRifle, position, 0.7f);
        }
    }

    private void Projectile_OnProjectileHit(object sender, System.EventArgs e)
    {
        Projectile projectile = (Projectile)sender;
        var type = projectile.type;
        var position = projectile.transform.position;

        if (type == Projectile.Type.RifleBullet || type == Projectile.Type.ShotgunBullet)
        {
            PlaySound(audioClipsSO.bulletHit, position);
        }
        else if (type == Projectile.Type.CannonBullet)
        {
            PlaySound(audioClipsSO.bulletHit, position, 0.3f);
        }
        else if (type == Projectile.Type.Arrow)
        {
            PlaySound(audioClipsSO.arrowHit, position);
        }
        else if (type == Projectile.Type.EnemyArrow)
        {
            PlaySound(audioClipsSO.arrowHit, position, 0.3f);
        }
    }

    private void Bow_OnBowRelease(object sender, System.EventArgs e)
    {
        Bow bow = sender as Bow;
        PlaySound(audioClipsSO.releaseBow, bow.transform.position);
    }

    private void MeleeWeapon_OnMeleeMissed(object sender, System.EventArgs e)
    {
        MeleeWeapon attackerWeapon = sender as MeleeWeapon;
        PlaySound(audioClipsSO.meleeWeaponMissed, attackerWeapon.transform.position);
    }

    private void MeleeWeapon_OnMeleeHit(object sender, System.EventArgs e)
    {
        MeleeWeapon attackerWeapon = sender as MeleeWeapon;
        PlaySound(audioClipsSO.meleeWeaponHit, attackerWeapon.transform.position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume * GeneralVolume * SfxVolume);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {

        AudioSource.PlayClipAtPoint(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume * GeneralVolume * SfxVolume);
    }

    public void PlayFootstepsSound(Vector3 position, float volume)
    {
        PlaySound(audioClipsSO.steps, position, volume);
    }
}
