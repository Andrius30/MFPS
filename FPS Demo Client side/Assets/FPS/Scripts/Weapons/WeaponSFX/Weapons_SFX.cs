using UnityEngine;

public class Weapons_SFX
{
    AudioSource source;
    AudioSource otherAudiosource;
    ClientWeapon baseWeapon;

    public Weapons_SFX(ClientWeapon baseWeapon)
    {
        this.baseWeapon = baseWeapon;
        source = baseWeapon.GetComponent<AudioSource>();
        otherAudiosource = baseWeapon.transform.Find("otherAudiosource").GetComponent<AudioSource>();
    }

    // =============== SHOOTING ===================================================================
    public virtual void PlayShootAudio() => source.PlayOneShot(baseWeapon.shootingSound);

    // =============== Other sounds ===================================================================
    public virtual void PlayAudio(AudioClip clip)
    {
        if (!otherAudiosource.isPlaying)
            otherAudiosource.PlayOneShot(clip);
    }
}
